using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Firebase;
using Firebase.Database;
using Firebase.Analytics;
using Firebase.Unity.Editor;

using Newtonsoft.Json;

public class FirebaseTest : MonoBehaviour {

	DatabaseReference reference;

	void Start() {
       

    }

    public void OnFirebaseInit()
    {
		// Set this before calling into the realtime database.
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://liarliargame-dc9f6.firebaseio.com/");


		// Get the root reference location of the database.
		reference = FirebaseDatabase.DefaultInstance.RootReference;

		Player p;
		List<Player> playersList = new List<Player>();

		playersList.Add(new Player("Player 1", true, 10));
		playersList.Add(new Player("Player 2", false, 5));
		playersList.Add(new Player("Player 3", false, 50));

		Dictionary<string, string> dic = new Dictionary<string, string>();
		dic.Add("a", "b");
		dic.Add("c", "d");
		dic.Add("uong", "aseionfr");

		string json = JsonConvert.SerializeObject(playersList);

		print(json);

		Debug.Log(json);

		/* reference.Child("test").SetRawJsonValueAsync(json).ContinueWith(task => {

			Debug.Log("Task isFaulted: " + task.IsFaulted);

		}); */

		//reference.Child("test").SetRawJsonValueAsync(json);
	}


    void Update()
    {

    }

}