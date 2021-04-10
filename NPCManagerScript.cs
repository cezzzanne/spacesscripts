using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Spaces {
    public class NPCManagerScript : MonoBehaviour {

        public List<Material> skins;

        public GameObject NPCPrefab;

        private List<GameObject> NPCList;

        public List<PathCreator> allPaths;

        public List<PathCreator> availablePaths;

        private Dictionary<int, List<PathCreator>> PathNetwork;

        int totalNPCs;

        public int currentActiveNPCs = 0;

        private List<string> usernames = new List<string>() {"waterbro0k", "sownds4", "jarndyce", "po1t", "weggg", "darbby", "bar3ad", "robb99", "mackin", "quinion",
        "bray", "swi1ls", "mowcher98", "chevy02", "anny", "ar3thusa", "smiveyyy", "peplow", "cryptoss", "adavis", "cparsons", "terlperk", "skewton"};

        private List<string> usernamesAvail;

        int currentTestPath = 6;

        // 1 -> 2, 7
        // 2 -> 3
        // 3 -> 4 , 8
        // 4 -> 1 , 5
        // 5 -> 6, 10
        // 6 -> 2, 7
        // 7 -> 4, 8
        // 8 -> 9
        // 9 -> 1, 5
        // 10 -> 9, 11
        // 11 -> 3

        void Start() {
            totalNPCs = allPaths.Count;
            availablePaths = new List<PathCreator>(allPaths);
            usernamesAvail = new List<string>(usernames);
            PathNetwork = new Dictionary<int, List<PathCreator>>();
            PathNetwork.Add(0, new List<PathCreator>(){allPaths[1], allPaths[6]});
            PathNetwork.Add(1, new List<PathCreator>(){allPaths[2]});
            PathNetwork.Add(2, new List<PathCreator>(){allPaths[3], allPaths[7]});
            PathNetwork.Add(3, new List<PathCreator>(){allPaths[0], allPaths[4]});
            PathNetwork.Add(4, new List<PathCreator>(){allPaths[5], allPaths[9]});
            PathNetwork.Add(5, new List<PathCreator>(){allPaths[1], allPaths[6]});
            PathNetwork.Add(6, new List<PathCreator>(){allPaths[3], allPaths[7]});
            PathNetwork.Add(7, new List<PathCreator>(){allPaths[8]});
            PathNetwork.Add(8, new List<PathCreator>(){allPaths[0], allPaths[4]});
            PathNetwork.Add(9, new List<PathCreator>(){allPaths[8], allPaths[10]});
            PathNetwork.Add(10, new List<PathCreator>(){allPaths[2]});
            SetUpNPCs();
        }

        void TestPath(int pathIndex) {
            PathCreator newPath = allPaths[pathIndex];
            GameObject character = Instantiate(NPCPrefab);
            character.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = skins[GetRandomInt(0, skins.Count - 1)];
            character.GetComponent<FollowPathScript>().SetPathCreator(newPath, pathIndex, this, "ss");

        }


        void SetUpNPCs() {
            int amountOfCharacters = GetRandomInt(0, totalNPCs - currentActiveNPCs);
            currentActiveNPCs += amountOfCharacters;
            for(int i = 0; i < amountOfCharacters; i++) {
                GameObject character = Instantiate(NPCPrefab);
                string username = usernamesAvail[GetRandomInt(0, usernamesAvail.Count - 1)];
                character.transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "@" + username;
                character.transform.GetChild(0).GetComponent<SkinnedMeshRenderer>().material = skins[GetRandomInt(0, skins.Count - 1)];
                PathCreator charPath = availablePaths[GetRandomInt(0, availablePaths.Count  - 1)];
                character.GetComponent<FollowPathScript>().SetPathCreator(charPath, allPaths.IndexOf(charPath), this, username);
                availablePaths.Remove(charPath);
                usernamesAvail.Remove(username);
            }
            StartCoroutine(RecurringNPCs());
        }

        int GetRandomInt(int floor, int ciel) {
            return Mathf.CeilToInt(Random.Range(floor, ciel));
        }

        IEnumerator RecurringNPCs() {
            int whenToAddNextBatch = GetRandomInt(40, 50);
            yield return new WaitForSeconds(whenToAddNextBatch);
            SetUpNPCs();
        }

        public void FinishPath(int pathIndex, FollowPathScript character, string username) {
            bool willKeepGoing = GetRandomInt(0, 10) < 6;
            availablePaths.Add(allPaths[pathIndex]);
            if (willKeepGoing) {
                // get a random path of the available paths that spring out of this path
                PathCreator newPath = PathNetwork[pathIndex][GetRandomInt(0, PathNetwork[pathIndex].Count - 1)];
                int newPathIndex = allPaths.IndexOf(newPath);
                availablePaths.Remove(newPath);
                character.SetPathCreator(newPath, newPathIndex, this, username);
            } else {
                currentActiveNPCs--;
                usernamesAvail.Add(username);
                Destroy(character.gameObject);
            }
        }
    }
}
