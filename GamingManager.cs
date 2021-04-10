using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;


namespace Spaces {
    public class GamingManager : MonoBehaviour {
        // Start is called before the first frame update
        int stepInGame = 0;
        int calls = 0;
        public string currentGameName = "";

        CharacterScript character;

        private string potentialGameCode = "";

        private bool joiningGame = false;

        private string navigationJS = "var board = document.getElementsByClassName('Table__board')[0]; var moveDiv = document.createElement('div'); moveDiv.style.zIndex = 9999999; moveDiv.style.top = board.style.top; moveDiv.style.left = board.style.left; moveDiv.style.position = 'absolute'; moveDiv.style.width = board.style.width; moveDiv.style.height = board.style.height; moveDiv.style.backgroundColor = '#ffffff45'; moveDiv.style.display = 'none'; document.body.appendChild(moveDiv); var bot = document.createElement('div'); bot.style.position = 'absolute'; bot.style.zIndex = 99999999; bot.style.width = '55px'; bot.style.height = '55px'; bot.style.left = '3px'; bot.style.top = '10px'; document.body.appendChild(bot); bot.style.backgroundColor = 'gray'; bot.style.borderRadius = '5px'; function position() { bot.style.left = (window.pageXOffset + 3) + 'px'; bot.style.top = (window.pageYOffset + 10) + 'px'; const size = (55 / ( window.outerWidth / window.innerWidth)) + 'px'; bot.style.height = size; bot.style.width = size; const fontSize = (45 / ( window.outerWidth / window.innerWidth)); pp.style.fontSize = fontSize + 'px';} document.addEventListener('scroll', position, false); var pp = document.createElement('p'); pp.style.textAlign = 'center';  pp.innerHTML = '👻'; pp.style.fontSize = '45px'; bot.appendChild(pp); var isMovingAround = false;  function moveAround() { if (isMovingAround) { moveDiv.style.display = 'none'; isMovingAround = false; pp.innerHTML = '👻' } else { moveDiv.style.display = 'block'; isMovingAround = true; pp.innerHTML =  '🔙'} }; bot.addEventListener('click', moveAround, false);";


        private string navigationJSV2 = "const botSize = screen.height / 14; const fontSize = screen.height / 28; var board = document.getElementsByClassName('Table__board')[0]; var moveDiv = document.createElement('div'); moveDiv.style.zIndex = 9999999; moveDiv.style.top = board.style.top; moveDiv.style.left = board.style.left; moveDiv.style.position = 'absolute'; moveDiv.style.width = board.style.width; moveDiv.style.height = board.style.height; moveDiv.style.backgroundColor = '#ffffff45'; moveDiv.style.display = 'none'; document.body.appendChild(moveDiv); var bot = document.createElement('div'); bot.style.position = 'absolute'; bot.style.zIndex = 99999999; bot.style.width = botSize + 'px'; bot.style.height = botSize + 'px'; bot.style.left = '3px'; bot.style.top = '10px'; document.body.appendChild(bot); bot.style.backgroundColor = 'gray'; bot.style.borderRadius = '5px'; function position() { bot.style.left = (window.pageXOffset + 3) + 'px'; bot.style.top = (window.pageYOffset + 10) + 'px'; const size = (botSize / ( window.outerWidth / window.innerWidth)) + 'px'; bot.style.height = size; bot.style.width = size; const newFontSize = (fontSize / ( window.outerWidth / window.innerWidth)); pp.style.fontSize = newFontSize + 'px';} document.addEventListener('scroll', position, false); var pp = document.createElement('p'); pp.style.textAlign = 'center';  pp.innerHTML = '👻'; pp.style.fontSize = fontSize + 'px'; bot.appendChild(pp); var isMovingAround = false;  function moveAround() { if (isMovingAround) { moveDiv.style.display = 'none'; isMovingAround = false; pp.innerHTML = '👻' } else { moveDiv.style.display = 'block'; isMovingAround = true; pp.innerHTML =  '🔙'} }; bot.addEventListener('click', moveAround, false); bot.style.display = 'flex'; bot.style.alignItems = 'center'; pp.style.flex = '1'; pp.style.textAlign = 'center'; bot.style.boxShadow = '2px 2px 2px #505050'; const exp = document.createElement('div'); document.body.appendChild(exp); exp.style.height = botSize + 'px'; exp.style.top = '10px'; exp.style.left = botSize + 3 + 3 + 'px'; exp.style.width = botSize * 5 + 'px'; exp.style.borderRadius = '5px'; exp.style.backgroundColor = 'white'; exp.style.boxShadow = '2px 2px 2px 2px white'; const etxt = document.createElement('p'); etxt.style.textAlign = 'center'; etxt.style.color = '#787878'; etxt.style.fontFamily = 'Arial'; exp.appendChild(etxt); etxt.style.fontWeight = 'bold'; etxt.style.fontSize = botSize / 5 + 'px'; exp.style.display = 'flex'; etxt.style.flex = '1'; exp.style.flex = '1'; exp.style.alignItems = 'center'; etxt.innerHTML = 'Welcome to ~games~ - use the bottom on the left to be able to zoom and move around the board!<br> ✨ click me to dissappear ✨ '; function hideSupportText() { exp.style.display = 'none'; }; exp.addEventListener('click', hideSupportText, false); exp.style.position = 'absolute';";
        // when we want to join a game
        public void SetPotentialGame(string gameCode) {
            potentialGameCode = gameCode;
        } 

        public void SelectGame(string name) {
            currentGameName = name;
            OpenGame(name);
        }

        void OpenGame(string gameName) {
            InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
            options.displayURLAsPageTitle = false;
            options.pageTitle = "👾 ~spaces~ 👾";
            options.hidesHistoryButtons = true;
            options.backButtonText = "Quit";
            options.pinchAndZoomEnabled = true;
            // ios: left, bottom, top, right
            InAppBrowser.EdgeInsets insets = new InAppBrowser.EdgeInsets(0, 50, 50, 0);
            options.insets = insets;
            InAppBrowser.OpenURL("https://playingcards.io/game/" + gameName, options);
        }

        public void SetCharacterScript(CharacterScript cs) {
            character = cs;
        }

        public void FinishedLoading(string url) {
            string command = "";
            string command2 = "var div = document.createElement('div'); div.style.zIndex = 1000002; div.style.position = 'absolute'; div.style.height = '100vh'; div.style.width = '100vw'; div.style.backgroundColor = 'white'; document.body.insertBefore(div, document.body.childNodes[0]);div.id = 'loadingDiv';";
            InAppBrowser.ExecuteJS(command2);
            if (stepInGame == 0) {
                StartCoroutine(CreateGame());
                // command = "var interval1 = setInterval(function(){ try { document.getElementsByClassName('Landing__cta CreateGame')[0].click(); console.log('worked1'); clearInterval(interval1);} catch { console.log('timeout1');}; }, 100);";
                // InAppBrowser.ExecuteJS("console.log('here');");
                // command = "document.getElementsByClassName('Landing__cta CreateGame')[0].click();";
                stepInGame++;
            } else if (stepInGame == 1) {
                command = "var interval = setInterval(function(){ try { document.getElementsByClassName('MenuBar')[0].style.display = 'None'; document.getElementsByClassName('ToolBoxToast')[0].style.display = 'None'; document.getElementsByClassName('prettyButton')[0].click(); document.getElementById('loadingDiv').style.display = 'none'; console.log('worked'); " + navigationJSV2 + " clearInterval(interval);} catch { console.log('timeout');}; }, 10);";
                stepInGame--;
                string gameCode = url.Split('/')[url.Split('/').Length - 1];
                character.SetGame(currentGameName, gameCode);
                // get from the url the game code
                // we need to set the collider from the beggining -> we use prc to send to our other characterScript instances that we are on a game - with a game name
                // when we collider with another characterscript we check if they are in game and their game name and their code. We enable button to join and then we join
            }
            InAppBrowser.ExecuteJS(command);
        }

        IEnumerator CreateGame() {
            yield return new WaitForSeconds(1.5f);
            string command = "var interval1 = setInterval(function(){ try { document.getElementsByClassName('Landing__cta CreateGame')[0].click(); console.log('worked1'); clearInterval(interval1);} catch { console.log('timeout1');}; }, 10);";
            InAppBrowser.ExecuteJS(command);
        }

        public void OnGameClosed() {
            character.LeftGame();
        }


        public void JoinGame() {
            stepInGame++;
            Debug.Log("zzzz potential code to join : " + potentialGameCode);
            character.JoinedGame();
            InAppBrowser.DisplayOptions options = new InAppBrowser.DisplayOptions();
            options.displayURLAsPageTitle = false;
            options.pageTitle = "👾 ~spaces~ 👾";
            options.hidesHistoryButtons = true;
            options.backButtonText = "Quit";
            options.pinchAndZoomEnabled = true;
            InAppBrowser.EdgeInsets insets = new InAppBrowser.EdgeInsets(0, 50, 50, 0);
            options.insets = insets;
            InAppBrowser.OpenURL("https://playingcards.io/" + potentialGameCode, options);
        }

    }
}