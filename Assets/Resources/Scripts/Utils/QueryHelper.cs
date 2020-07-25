using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;
using Newtonsoft.Json;
using Firebase.Firestore;
using System.Threading.Tasks;
using System.Linq;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using Firebase.Extensions;

public class QueryHelper : MonoBehaviour
{

    //  DEBUG ONLY
    private string customIdea;
    //  DEBUG ONLY

    public bool truthOrLie;

    private bool lewdnessAllowed;

    private FirebaseFirestore fs;

    GP2_TEXT_ENTRY gp2;

    private CollectionReference categoriesRef;

    private CollectionReference ideasRef;

    public QueryHelper()
    {

    }

    public void SetVars(bool truthOrLie, FirebaseFirestore firestoreReference, GP2_TEXT_ENTRY gp2)
    {
        this.truthOrLie = truthOrLie;
        this.fs = firestoreReference;
        this.gp2 = gp2;
        this.lewdnessAllowed = true;

        this.ideasRef = fs.Collection("ideas");
        this.categoriesRef = fs.Collection("categories");
    }

    public void SetLewdnessAllowed(bool lewdnessAllowed)
    {
        this.lewdnessAllowed = lewdnessAllowed;
    }

    public void BeginQuery()
    {
        if ((this.customIdea == null))
        {
            this.RunQuery();
        }
        else
        {
            this.onTextContentExtracted(this.customIdea);
        }

    }

    System.Random rand = new System.Random();

    private void RunQuery()
    {
        double randomNumber;
        randomNumber = rand.NextDouble();

        //  Create a query against the collection
        Firebase.Firestore.Query query = ideasRef
            .WhereEqualTo("trueiftruth", this.truthOrLie)
            .WhereLessThan("random", randomNumber)
            .OrderByDescending("random")
            .Limit(1);


        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            onIdeasQueryComplete(task);
        });
            //.addOnCompleteListener
            //(new OnCompleteListener<QuerySnapshot>());
    }

    private void onIdeasQueryComplete(Task<QuerySnapshot> task)
    {
        string text;
        if ((task.IsCompleted && !task.IsFaulted
                    && (task.Result.Documents.Count<DocumentSnapshot>() > 0)))
        {
            //  Get content
            text = ((string)(task.Result.Documents.First<DocumentSnapshot>().ToDictionary()["content"]));

            Debug.Log( ("onIdeasQueryComplete: Query outcome: " + text));

            //  Get lewd
            bool isLewd = ((bool)(task.Result.Documents.First<DocumentSnapshot>().ToDictionary()["lewd"]));
            if ((isLewd
                        && !this.lewdnessAllowed))
            {
                Debug.Log( "onIdeasQueryComplete: LEWD - rerunning");
                this.RunQuery();
            }
            else
            {
                this.onTextContentExtracted(text);
            }

        }
        else
        {
            Debug.Log( ("onIdeasQueryComplete: Query FAILED: " + task.Exception.Message + ", " + task.Exception.StackTrace + ", " + task.Exception.HelpLink));
            //  callQueryMethod();
        }

    }

    private void onTextContentExtracted(string text)
    {
        //  Check if querying required
        Match mCat = checkForCategories(text);

        //  If categories exist, we have the mCat to go forward...
        if (mCat.Success)
        {
            Debug.Log("PATTERN FOUND");
            this.resolveNextCategory(text, mCat);
        }
        else
        {
            Debug.Log("PATTERN NOT FOUND");
            this.finalFormatting(text);
        }

        //  TODO: Implement lewdness filter
        //  Boolean isLewd = (Boolean) task.getResult().getDocuments().get(0).get("true_if_truth");
    }

    private Match checkForCategories(string text)
    {
        //  Figure out category patterns
        //Pattern patternCat = Pattern.compile("(?<=\\\\[)([^\\\\]]+)(?=\\\\])");
        //return patternCat.Match(text);

        string pattern = @"(?<=\[)([^\]]+)(?=\])";
        return Regex.Match(text, pattern);
    }

    private void resolveNextCategory(string text, Match Match)
    {
        string newText;

        try
        {
            Match mCat = Match;
            //  Extract category content
            string allSubCats = text.Substring(mCat.Index, mCat.Length);

            Debug.Log("ALLSUBCATS: " + allSubCats);

            //  If this category is singular
            if (!allSubCats.Contains("/"))
            {
                // Debug.Log("resolveNextCategory: LEWD - rerunning");
                this.queryCategories(text, allSubCats, mCat);
            }
            else
            {
                string[] allSubCatsSplit = allSubCats.Split('/');
                int randomIndex = ((int)((rand.NextDouble() * ((double)(allSubCatsSplit.Length)))));
                //  Choose random category
                string randomCategory = allSubCatsSplit[randomIndex];
                //  Replace original multi-category with its single chosen one

                //text = text.Replace(allSubCats, randomCategory);
                newText = text.Remove(mCat.Index, mCat.Length).Insert(mCat.Index, randomCategory);

                //  Query for this category
                this.queryCategories(newText, randomCategory, mCat);
            }
        }
        catch (Exception e)
        {
            Debug.Log("resolveNextCategory ERROR: " + e.Message);
        }



    }

    private void queryCategories(string text, string category, Match mCat)
    {
        Debug.Log(("queryCategories: QUERYING "
                        + (category + (" FOR TEXT " + text))));
        double randomNumber;
        randomNumber = rand.NextDouble();

        //  Create a query against the collection
        Firebase.Firestore.Query query = categoriesRef
            .WhereEqualTo("category", category)
            .WhereLessThan("random", randomNumber)
            .OrderByDescending("random")
            .Limit(1);
        query.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            onCatQueryComplete(task, text, category, mCat);
        });
    }

    void onCatQueryComplete(Task<QuerySnapshot> task, string text, string category, Match mCat)
    {
        try
        {
            Debug.Log(1);
            if ((task.IsCompleted && !task.IsFaulted
                    && (task.Result.Documents.Count<DocumentSnapshot>() > 0)))
            {

                string resultText = ((string)(task.Result.Documents.First<DocumentSnapshot>().ToDictionary()["entry"]));

                bool isLewd = ((bool)(task.Result.Documents.First<DocumentSnapshot>().ToDictionary()["lewd"]));

                if (isLewd && !lewdnessAllowed)
                {
                    Debug.Log("LEWD BLOCKED: TRY AGAIN");
                    queryCategories(text, category, mCat);
                }
                else
                {

                    string newText = text.Remove(mCat.Index - 1, category.Length + 2).Insert(mCat.Index - 1, resultText);

                    // If matcher has another match, resolve next category
                    // else, check again for matches

                    //if (matcher.find()) resolveNextCategory(newText, matcher);
                    //else
                    //{
                    Match newMCat = checkForCategories(newText);
                    if (newMCat.Success)
                    {
                        Debug.Log("onComplete: cat: " + category + ", entry:" + newText);
                        resolveNextCategory(newText, newMCat);
                    }
                    else
                    {
                        Debug.Log(10);
                        Debug.Log("onComplete: cat: " + category + ", entry:" + newText);
                        finalFormatting(newText);
                    }
                }

            }


            else
            {
                if (task.Result.Documents.Count<DocumentSnapshot>() == 0)
                {
                    Debug.Log("CATEGORY QUERY: NO RETURN: TRY AGAIN");

                    queryCategories(text, category, mCat);
                }
                else
                {
                    try
                    {
                        Debug.Log("onComplete: Query FAILED: " + task.Exception.Message);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("onComplete: ERROR IN QUERY FAILED: " + e.Message);
                    }
                }
                // TODO: Act upon query fail
            }

        }
        catch (Exception e)
        {
            Debug.Log(e.Message + ", " + e.StackTrace);
        }


    }


    private void finalFormatting(string text)
    {
        string newText = text;

        try
        {
            newText = dealWithNumbers(newText);
            newText = dealWithArticle(newText);
            newText = capitalise(newText);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message + ", " + e.Source + ", " + e.StackTrace);
        }

        this.gp2.SetHintText(newText);
    }

    private string dealWithNumbers(string text)
    {
        string newText = text;
        //  DEAL WITH NUMBERS
        string patternNum = @"(?<=<)([^>]+)(?=>)";
        MatchCollection matches = Regex.Matches(text, patternNum);
        Match mNum = matches.Count > 0 ? matches[0] : null;

        while (mNum != null)
        {
            string numbers = newText.Substring(mNum.Index, mNum.Length);
            string[] numbersSplit = numbers.Split('-');
            int[] n = new int[2];
            for (int i = 0; i < 2; i++)
            {
                n[i] = int.Parse(numbersSplit[i]);
            }

            double randomNumber = rand.NextDouble();
            int number = (n[0] + ((int)((randomNumber
                        * (((double)(n[1])) - n[0])))));

            Debug.Log(("formatCategory: NUMBERSPLIT: " + numbersSplit));
            Debug.Log(("formatCategory: NUMBERS: " + numbers));
            Debug.Log(("formatCategory: NUMBER: " + number));

            //newText = text.replaceFirst(("<"
            //                + (numbers + ">")), Integer.toString(number));

            newText = newText.Remove(mNum.Index - 1, numbers.Length + 2).Insert(mNum.Index - 1, number.ToString());

            matches = Regex.Matches(newText, patternNum);
            mNum = matches.Count > 0 ? matches[0] : null;
        }

        return newText;
    }

    private string dealWithArticle(string text)
    {
        string newText = text;
        //  DEAL WITH NUMBERS
        string patternArt = @"@";

        MatchCollection matches = Regex.Matches(newText, patternArt);
        Match mArt = matches.Count > 0 ? matches[0] : null;

        while (mArt != null)
        {
            Debug.Log("dealWithArticle: mART FOUND AT "+ mArt.Index+" IN "+newText);
            char charAfter = ' ';
            int incr = 0;
            while ((charAfter == ' '))
            {
                incr++;
                if ((mArt.Index
                            + incr
                            > (newText.Length - 1)))
                {
                    Debug.Log("dealWithArticle: INDEX EXCEEDED BOUNDS AT "+ mArt.Index + incr);
                    return newText;
                }
                else
                {
                    charAfter = text[(mArt.Index + incr)];
                    Debug.Log(("dealWithArticle: CHAR NOW: '" + charAfter + "'"));
                }

            }

            string vowelSet = "aeiouAEIOU";
            string article;
            //  If not a vowel...
            if ((vowelSet.IndexOf(charAfter) == -1))
            {
                article = "a";
            }
            else
            {
                article = "an";
            }

            Debug.Log(("dealWithArticle: @ to change to " + article));
            newText = newText.Remove(mArt.Index, 1).Insert(mArt.Index, article);

            // Re-search for matches
            matches = Regex.Matches(newText, patternArt);
            mArt = matches.Count > 0 ? matches[0] : null;
        }

        return newText;
    }

    private string capitalise(string text)
    {
        //  TODO: So far only capitalises first letter - potential for capitalising elsewhere where necessary? i.e. after punctuation
        string newText = text;

        //  Capitalise first letter only
        newText = char.ToUpper(newText[0]) + newText.Substring(1);
        return newText;
    }

    public void setCustomIdea(string customIdea)
    {
        this.customIdea = customIdea;
    }

   
}