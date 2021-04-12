using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;

namespace Spaces {
    public class TreasureHuntScript : MonoBehaviour {
        // Start is called before the first frame update

        public GameObject TreasurePrefab;

        public TMPro.TextMeshProUGUI question;

        public TMPro.TextMeshProUGUI answer;

        private string questionAnswer;

        public GameObject QuestionModal, AnsweredCorrectly, AnsweredIncorrectly;

        public ItemLoaderStore itemLoader;

        private GameObject Treasure;

        string questionText = "-1";

        string[] pos;

        void Start() {
            // StartCoroutine(WaitToCreateTreasureHunt()); // to not load sooo many things at the beggining
            GetTreasureHunt();
            StartCoroutine(SetQuestion());
        }

        IEnumerator WaitToCreateTreasureHunt() {
            yield return new WaitForSeconds(8);
            GetTreasureHunt();
        }

        public void GetTreasureHunt() {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("treasure").GetValueAsync().ContinueWith(task => {
                DataSnapshot snapshot = task.Result;
                if (!snapshot.Exists) {
                    questionText = "0";
                    return;
                }
                Dictionary<string, object> data = snapshot.Value as Dictionary<string, object>;
                string positions = data["pos"] as string;
                string answer = data["answer"] as string;
                questionAnswer = answer;
                pos = positions.Split(';');
                questionText = data["question"] as string;
            });
        }

        IEnumerator SetQuestion() {
            while(questionText == "-1") {
                yield return null;
            }
            if (questionText != "0") {
                question.text = "Well done! Now ... for $200 .. one last question: \n " + questionText;
                print("cccc pos " + pos);
                Vector3 chestPos = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
                Treasure = Instantiate(TreasurePrefab) as GameObject;
                Treasure.transform.position = chestPos;
                Treasure.GetComponent<TreasureScript>().SetHuntManager(this); 
            }
        }

        public void ShowQuestion() {
            QuestionModal.SetActive(true);
        }

        public void HideQuestion() {
            QuestionModal.SetActive(false);
        }

        public void FinishHunt() {
            AnsweredCorrectly.SetActive(false);
        }

        public void AnswerQuestion() {
            AnsweredIncorrectly.SetActive(false);
            print("cccc answer lengths " + questionAnswer.Trim().ToLower().Length + " " + answer.text.Trim().ToLower().Length);
            print("cccc answer vals " + questionAnswer.Trim().ToLower() + " " + answer.text.Trim().ToLower());
            if (answer.text.Trim().ToLower().Contains(questionAnswer.Trim().ToLower())) {
                Destroy(Treasure);
                QuestionModal.SetActive(false);
                AnsweredCorrectly.SetActive(true);
                itemLoader.UpdateCoinsText(itemLoader.coinsValue + 200);
                itemLoader.UpdateFirebaseCoins(itemLoader.coinsValue + 200);
                RemoveTreasureFirebase();
            } else {
                AnsweredIncorrectly.SetActive(true);
            }
        }

        void RemoveTreasureFirebase() {
            DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
            reference.Child("treasure").RemoveValueAsync();
        }
    }
}
