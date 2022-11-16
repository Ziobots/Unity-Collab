/*******************************************************************************
* Name : leaderSetup.cs
* Section Description : This code handles the leaderboard UI.
* -------------------------------
* - HISTORY OF CHANGES -
* -------------------------------
* Date		Software Version	Initials		Description
* 11/16/22  0.10                 DS              Made the thing
*******************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class leaderSetup : MonoBehaviour
{
    // Base Data Stuff
    public GameObject dataManager;
    [HideInInspector] public sharedData dataInfo;
    
    // ui stuff
    public GameObject transitioner; 
    public GameObject playMenu;

    // leaderboard variables
    public GameObject statHolder;
    public GameObject leaderHolder;
    public GameObject leaderPrefab;
    public GameObject loadHolder;
    public Scrollbar scroller;
    public GameObject rankField;

    public bool menuActive = false;

    // error variables
    public GameObject errorMenu;
    private Color32 errorColor = new Color32(253,106,106,255);

    private void setStatValue(string statName, string statValue){
        Transform statObj = statHolder.transform.Find(statName);
        if (statObj != null && statObj.gameObject != null){
            statObj.Find("valueField").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = statValue;
        }
    }

    public void setupMenu(){
        // Get data management script
        if (dataManager != null){
            dataInfo = dataManager.GetComponent<sharedData>();
        }
    }

    public void wipeLeaderboard(){
        foreach (Transform child in leaderHolder.transform.Find("gridContent")){
            Destroy(child.gameObject);
        }
    }

    private IEnumerator doWait(System.Action onComplete,float waitTime){
        yield return new WaitForSecondsRealtime(waitTime);
        // run on complete
        onComplete();
    }

    // tween functions
    private void setTransparency(float alpha){
        if (loadHolder){
            loadHolder.GetComponent<CanvasGroup>().alpha = alpha;
        }
    }

    public void loadLeaderboard(){
        if (leaderHolder != null){
            print("Load leader menu");

            float waitTime = 0f;
            if ((Time.realtimeSinceStartup - dataInfo.lastLeaderboardTime >= dataInfo.resetLeaderTime) || !dataInfo.connectedToPlayfab){
                print("LOAD WAIT");
                wipeLeaderboard();
                loadHolder.SetActive(true);
                LeanTween.cancel(loadHolder);
                loadHolder.GetComponent<CanvasGroup>().alpha = 1f;
                rankField.GetComponent<CanvasGroup>().alpha = 0f;
                waitTime = 2f;
            }else{
                loadHolder.SetActive(!dataInfo.connectedToPlayfab);
            }

            scroller.value = 1f;

            dataInfo.onRankGet = delegate{
                if (!(gameObject.activeSelf && menuActive && dataInfo.connectedToPlayfab)){
                    return;
                }

                string myRank = "Unknown";
                if (dataInfo.clientLeaderboardData != null && dataInfo.clientLeaderboardData.Leaderboard.Count > 0){
                    if (rankField && dataInfo.clientLeaderboardData.Leaderboard[0] != null){
                        myRank = "#" + (dataInfo.clientLeaderboardData.Leaderboard[0].Position + 1);
                    }
                }

                rankField.transform.Find("title").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = myRank;
                rankField.GetComponent<TMPro.TextMeshProUGUI>().text = myRank;
                LeanTween.cancel(rankField);

                if (waitTime == 0){
                    rankField.GetComponent<CanvasGroup>().alpha = 1f;
                }else{
                    LeanTween.value(rankField,0f,1f,0.1f).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(delegate(float alpha){
                        rankField.GetComponent<CanvasGroup>().alpha = alpha;
                    });
                }
            };

            dataInfo.onLeaderGet = delegate{
                if (!dataInfo.connectedToPlayfab){
                    errorMenu.GetComponent<errorPopup>().displayError("Unable to connect to PlayFab.",errorColor);
                }
                
                if (!(gameObject.activeSelf && menuActive && dataInfo.connectedToPlayfab)){
                    return;
                }

                if (loadHolder.GetComponent<CanvasGroup>().alpha >= 1f){
                    LeanTween.cancel(loadHolder);
                    LeanTween.value(loadHolder,1f,0f,0.1f).setIgnoreTimeScale(true).setEaseLinear().setOnUpdate(setTransparency);
                }

                Dictionary<GameObject,bool> safeList = new Dictionary<GameObject, bool>();
                
                if (dataInfo.leaderboardData != null && dataInfo.connectedToPlayfab){
                    // fake values just for testing // REMOVE IT LATER
                    for (int a = 1; a <= 50; a++){
                        var fake = new PlayFab.ClientModels.PlayerLeaderboardEntry();
                        fake.Position = a;
                        fake.DisplayName = "Player " + a;
                        fake.StatValue = 1000 - (a*5);
                        dataInfo.leaderboardData.Leaderboard.Add(fake);
                    }

                    foreach (var item in dataInfo.leaderboardData.Leaderboard){
                        Transform newLeaderSpot = leaderHolder.transform.Find("gridContent").Find("leaderSpot_" + item.Position);

                        if (newLeaderSpot == null){
                            newLeaderSpot = Instantiate(leaderPrefab,leaderHolder.transform.Find("gridContent")).transform;
                        }

                        // Setup the tag 
                        if (newLeaderSpot != null){
                            newLeaderSpot.name = "leaderSpot_" + item.Position;

                            string displayName = item.DisplayName;
                            if (displayName == ""){
                                displayName = "Anonymous";
                            }

                            newLeaderSpot.Find("Holder").Find("username").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = displayName;
                            newLeaderSpot.Find("Holder").Find("rank").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "" + (item.Position + 1);
                            newLeaderSpot.Find("Holder").Find("score").gameObject.GetComponent<TMPro.TextMeshProUGUI>().text = "" + item.StatValue;
                            newLeaderSpot.Find("Holder").GetComponent<RectTransform>().sizeDelta = leaderHolder.transform.Find("gridContent").gameObject.GetComponent<GridLayoutGroup>().cellSize;

                            // add tag to safelist 
                            safeList.Add(newLeaderSpot.gameObject,true);

                            // tween effect
                            LeanTween.cancel(newLeaderSpot.gameObject);
                            if (waitTime == 0f){
                                newLeaderSpot.Find("Holder").gameObject.GetComponent<RectTransform>().pivot = new Vector2(0.5f,0.5f);
                            }else{
                                newLeaderSpot.Find("Holder").gameObject.GetComponent<RectTransform>().pivot = new Vector2(2f,0.5f);
                                LeanTween.value(newLeaderSpot.gameObject,2f,0.5f,0.5f).setIgnoreTimeScale(true).setEaseOutBack().setOnUpdate(delegate(float value){
                                    newLeaderSpot.Find("Holder").gameObject.GetComponent<RectTransform>().pivot = new Vector2(value,0.5f);
                                }).setDelay(item.Position / 10f);
                            }
                        }
                    }
                }else{
                    // there is no leaderboard data
                }

                // remove tags that dont exist
                int tagCount = 0;
                foreach (Transform child in leaderHolder.transform.Find("gridContent")){
                    if (!safeList.ContainsKey(child.gameObject)){
                        Destroy(child.gameObject);
                    }else{
                        tagCount++;
                    }
                }

                // update the size of the content field based on num of leaders
                RectTransform rect = leaderHolder.GetComponent<RectTransform>();
                if (rect){
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x,tagCount * 110);
                }
            };

            StartCoroutine(doWait(delegate{
                if (gameObject.activeSelf && menuActive){
                    dataInfo.getClientRank();
                    dataInfo.getLeaderboard();
                }
            },waitTime));
        }
    }

    public void backButton(){
        if (menuActive){
            menuActive = false;
            transitioner.GetComponent<fadeTransition>().startFade(delegate{
                unloadMenu();
                playMenu.SetActive(true);
                playMenu.GetComponent<playscreenSetup>().loadMenu();
            },false);
        }
    }

    public void loadMenu(){
        setupMenu();
        
        // load in the player stats
        if (dataInfo != null){
            setStatValue("stat_Score","" + dataInfo.statHighscore);
            setStatValue("stat_Run","" + dataInfo.statRunCount);
            setStatValue("stat_Win","" + dataInfo.statWinCount);
            setStatValue("stat_Enemy","" + dataInfo.statKillCount);
            setStatValue("stat_Perk","" + dataInfo.statPerkCount);
            setStatValue("stat_Room","" + dataInfo.statRoomCount);
        }

        menuActive = true;

        // load in the leaderboard
        loadLeaderboard();
    }

    public void unloadMenu(){
        menuActive = false;
        gameObject.SetActive(false);
        errorMenu.GetComponent<errorPopup>().hideError();
        dataInfo.onLeaderGet = null;
        dataInfo.onRankGet = null;
    }

    private void Update() {
        if (loadHolder != null){
            GameObject icon = loadHolder.transform.Find("icon").gameObject;
            GameObject label = loadHolder.transform.Find("textField").gameObject;
            if (icon != null && label != null){
                icon.GetComponent<RectTransform>().rotation = Quaternion.Euler(0,(Time.time * 120f) % 360f,0);
                icon.GetComponent<RectTransform>().pivot = new Vector2(0.5f,Mathf.Sin(Time.time * 5f) * 0.1f + 0.5f);
                label.GetComponent<RectTransform>().pivot = new Vector2(0.5f,Mathf.Sin(Time.time * 5f) * 0.1f + 0.5f);
            }
        }
    }
}
