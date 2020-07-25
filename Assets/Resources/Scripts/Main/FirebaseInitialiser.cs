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
//using System.Threading.Tasks;
using Firebase.Extensions;

public class FirebaseInitialiser : MonoBehaviour
{

    MainScript main;

    int mode;
    public static int MODE_FIRST = 0;
    public static int MODE_REINIT = 1;

    public DatabaseReference databaseRef;
    public FirebaseFirestore fs;

    public string errorMessage = "";


    public FirebaseInitialiser()
	{
	}

    public void BeginInitialising(MainScript main, int mode)
	{
         FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
                {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                var dependencyStatus = task.Result;
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {

                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    // Crashlytics will use the DefaultInstance, as well;
                    // this ensures that Crashlytics is initialized.
                    Firebase.FirebaseApp app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here for indicating that your project is ready to use Firebase. - Flag will simply be the existence of the reference


                    // Set this before calling into the realtime database.
                    FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://liarliargame-dc9f6.firebaseio.com/");

                    // Get the root reference location of the database.
                    this.databaseRef = FirebaseDatabase.DefaultInstance.RootReference;
                    this.fs = FirebaseFirestore.GetInstance(app);

                        if (this.mode == MODE_FIRST) main.OnFirebaseInitComplete(true);
                        else if (this.mode == MODE_REINIT) main.OnFirebaseReinitComplete(true);
                    }
                else
                {
                    errorMessage = "Could not resolve all Firebase dependencies: {0}";

                    UnityEngine.Debug.LogError(System.String.Format(
                      errorMessage, dependencyStatus));
                        // Firebase Unity SDK is not safe to use here.

                        if (this.mode == MODE_FIRST) main.OnFirebaseInitComplete(false);
                        else if (this.mode == MODE_REINIT) main.OnFirebaseReinitComplete(false);
                    }
            }
                //, TaskScheduler.FromCurrentSynchronizationContext()
                );
	}
   

}
