using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameLoader : MonoBehaviour {
  

    [SerializeField]
    private int scene;
    
    [SerializeField]
    private Text loadingText;
    
    [SerializeField]
    private Text titleText;
    
    [SerializeField]
    private Text title2Text;
    
    [SerializeField]
    private Text nameText;
    
    [SerializeField]
    private float scaleTitleFactor;
    
    private bool isLoadScene;
    private bool isStudioView;
    private int initialFontSizeTitle1;
    private int endFontSizeTitle1;
    private float fontAcc;
    
    
    void Update() {
       
        if (isLoadScene) {
            loadingText.color = new Color(loadingText.color.r, loadingText.color.g, loadingText.color.b, Mathf.PingPong(Time.time, 1));

        }

        if (isStudioView)
        {
            fontAcc = Mathf.Lerp(fontAcc, endFontSizeTitle1, scaleTitleFactor*Time.deltaTime);
            titleText.fontSize = (int) fontAcc;
        }
          
        
    }


    private void Start()
    {
        fontAcc = initialFontSizeTitle1;
        endFontSizeTitle1 = titleText.fontSize;
        titleText.fontSize =initialFontSizeTitle1;
        nameText.gameObject.SetActive(false);
        StartCoroutine(Scenario());
    }


    
    IEnumerator Scenario()
    {
        yield return new WaitForSeconds(1);
        titleText.text = "PositiveGames Studio представляет";
        isStudioView = true;
        yield return new WaitForSeconds(3);
        isStudioView = false;
        yield return new WaitForSeconds(2);
        nameText.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        title2Text.text = "История рядового посланника природы...";
        loadingText.text = "Загрузка...";
        isLoadScene = true;

        //ждем загрузки менеджеров
        while (!Managers.isManagersLoadingDone)
        {
            yield return null;
        }
        
        yield return new WaitForSeconds(3);//убрать в реальном коде(задержка для теста)
        AsyncOperation async = Managers.App.LoadMainMenu();
        while (!async.isDone) {
            //здесь можно прогресс показывать используя  async.progress
            yield return null;
        }

    }

}