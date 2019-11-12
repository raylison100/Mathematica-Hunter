using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Facebook.Unity;
using PlayFab;
using PlayFab.ClientModels;
using Facebook.MiniJSON;

public class HighScore : MonoBehaviour
{
    //HighScore

    Transform entryContainer;
    Transform entryTemplate;
    List<Transform> highscoreEntryTransformList;

    private void loadTransform()
    {

        entryContainer = transform.Find("highscoreEntryContainer");
        entryTemplate = entryContainer.Find("highscoreEntryTemplate");

        entryTemplate.gameObject.SetActive(false);

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            Debug.Log("Initializing table with default values...");
            //AQUi      
            
            // Reload
            jsonString = PlayerPrefs.GetString("highscoreTable");
            highscores = JsonUtility.FromJson<Highscores>(jsonString);
        }

        // Sort entry list by Score
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    // Swap
                    HighscoreEntry tmp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = tmp;
                }
            }
        }

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highscoreEntry, entryContainer, highscoreEntryTransformList);
        }
    }

    private void CreateHighscoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 31f;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            default:
                rankString = rank + "TH"; break;

            case 1: rankString = "1ST"; break;
            case 2: rankString = "2ND"; break;
            case 3: rankString = "3RD"; break;
        }

            entryTransform.Find("posText").GetComponent<Text>().text = rankString;

        int score = highscoreEntry.score;

        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Highlight First
        if (rank == 1)
        {
            entryTransform.Find("posText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        transformList.Add(entryTransform);
    }

    private void AddHighscoreEntry(int score, string name)
    {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { score = score, name = name };

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores == null)
        {
            // There's no stored table, initialize
            highscores = new Highscores()
            {
                highscoreEntryList = new List<HighscoreEntry>()
            };
        }

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }


    //Represents a single High score entry

    [System.Serializable]
    private class HighscoreEntry
    {
        public int score;
        public string name;
    }


    public void Awake()
    {
        //Verifica se o usuário está logado pois só é possível carregar os dados do Leadeboard caso ele esteja logado.
        if( FacebookAndPlayFabInfo.isLoggedOnPlayFab )
        {
            //Configura a chamada para carregar os dados do Leaderboard. Informa qual é a o nome do Leaderboard e quantos
            //resultados deve trazer no máximo.
            PlayFab.ClientModels.GetLeaderboardRequest request = new PlayFab.ClientModels.GetLeaderboardRequest();
            request.MaxResultsCount = 5;
            request.StatisticName = "Score";
 
            //Atualiza a váriavel para falso indicando que os dados ainda não foram carregados.
            FacebookAndPlayFabInfo.leaderboardLoaded = false;
            //Atualiza a váriavel para verdadeiro indicando que os dados estão sendo carregados.
            FacebookAndPlayFabInfo.leaderboardIsLoading = true;
 
            //As variáveis acima podem ser usadas em outros scripts que precisem esperar até os dados serem carregados ou 
            //identificar quando começou a carregar para realizar alguma ação dentro do seu jogo.
 
            //Utiliza o SDK do PlayFab para coletar os dados. Informando a chamada e a função de callback de sucesso e erro.
            PlayFabClientAPI.GetLeaderboard(request, GetLeaderboardSucessCallback, null);
        }
        else
        {
            Debug.Log("Não é possível carregar os Dados do Leadeboard sem está logado.");
        }
    }
 
    private void GetLeaderboardSucessCallback(PlayFab.ClientModels.GetLeaderboardResult result)
    {
        //Verifica se o resultado não veio nulo.
        if( result != null && result.Leaderboard != null )
        {
            //Pega o objeto que representa o Leaderboard.
            GameObject leaderboardGameObject = GameObject.Find("Leadeboard");
 
            //Percorre cada uma das 5 linhas que podem vir no Leaderboard e atualiza os campos na cena da Unity
            foreach (PlayFab.ClientModels.PlayerLeaderboardEntry leadeboardLine in result.Leaderboard)
            {
                GetUserInfoAndUpdateLeaderboard(leadeboardLine, leaderboardGameObject, result);  
            }

            loadTransform();
        }
    }
 
    private void GetUserInfoAndUpdateLeaderboard( PlayFab.ClientModels.PlayerLeaderboardEntry leaderBoardLine, GameObject leaderboardGameObject, GetLeaderboardResult result )
    {
        //Coleta o id do Facebook do usuário que está no rank através do Display Name que atualizamos ao usuário efetuar o login.
        string userFacebookId = leaderBoardLine.DisplayName;
 
        //Utiliza o SDK do Facebook para coletar as informações do usuário. O valor "/" + userFacebookId serve
        //para indicar de qual usuário queremos os dados. O valor HttpMethod.GET indica que a nossa chamada ao
        //facebook tem a intenção de somente coletar dados.
        FB.API("/" + userFacebookId, HttpMethod.GET,
        //Esta é uma outra forma de criar uma função de callback, o userInfoResult é a váriavel que
        //recebe o resultado que é passado para a função de callback
        (userInfoResult) =>
        {
            //Caso o resultado seja nulo, deu algum erro ao coletar os dados.
            if (userInfoResult == null)
            {
                Debug.Log("Não foi possível coletar os dados do usuário no Facebook.");
 
                //Verifica se está no ultimo registro do Leadeboard. Para indicar que finalizou o carregamento.
                if (leaderBoardLine.Position + 1 == result.Leaderboard.Count)
                {
                    //Atualiza a váriavel para verdadeiro indicando que os dados ainda já foram carregados.
                    FacebookAndPlayFabInfo.leaderboardLoaded = true;
                    //Atualiza a váriavel para falso indicando que os dados estão já foram carregados.
                    FacebookAndPlayFabInfo.leaderboardIsLoading = false;
 
                   Debug.Log("Os dados do Leaderboard foram carregados.");
                }
 
                return;
            }
 
            //Verifica se o retorno não foi um erro, ou algum tipo de cancelamento caso não seja nenhum desses tipos indica
            //que foi possível coletar os dados do facebook com sucesso.
            if (string.IsNullOrEmpty(userInfoResult.Error) && !userInfoResult.Cancelled && !string.IsNullOrEmpty(userInfoResult.RawResult))
            {
                Debug.Log("Sucesso ao coletar os dados da conta do usuário no Facebook");
 
                try
                {
                    //A resposta do Facebook vem em formato de Json e com isso nós convertemos o Json para um Dictionary
                    //para ser mais facil de coletar os dados
                    Dictionary<string, object> dict = Json.Deserialize(userInfoResult.RawResult) as Dictionary<string, object>;
                    string userFacebookName = dict["name"] as string;

                    //Pega o objeto que possui as informações de um jogador no Leaderboard.
                    //GameObject playerLeadeboardGameObject = leaderboardGameObject.transform.FindChild("Player (" + (leaderBoardLine.Position + 1) + ")").gameObject;

                    //Atualiza o texto com a posiçao do jogador no Leaderboard.
                    //playerLeadeboardGameObject.transform.FindChild("Position").GetComponent<Text>().text = (leaderBoardLine.Position + 1).ToString();

                    //Atualiza o texto com o nome do jogador no Facebook.
                    //playerLeadeboardGameObject.transform.FindChild("Name").GetComponent<Text>().text = userFacebookName;

                    //Atualiza o texto com o valor do Score do jogador no Leaderboard.
                    //playerLeadeboardGameObject.transform.FindChild("Score").GetComponent<Text>().text = leaderBoardLine.StatValue.ToString();

                    Debug.Log(leaderBoardLine.StatValue);
                    Debug.Log(userFacebookName);

                    AddHighscoreEntry(leaderBoardLine.StatValue, userFacebookName);

                    //Verifica se está no ultimo registro do Leadeboard. Para indicar que finalizou o carregamento.
                    if (leaderBoardLine.Position + 1 == result.Leaderboard.Count)
                    {
                        //Atualiza a váriavel para verdadeiro indicando que os dados ainda já foram carregados.
                        FacebookAndPlayFabInfo.leaderboardLoaded = true;
                        //Atualiza a váriavel para falso indicando que os dados estão já foram carregados.
                        FacebookAndPlayFabInfo.leaderboardIsLoading = false;
 
                       Debug.Log("Os dados do Leaderboard foram carregados.");
                    }
                }
                //Usado caso o Facebook não tenha retornado o id ou o nome do usuário.
                catch (KeyNotFoundException e)
                {
                    //Verifica se está no ultimo registro do Leadeboard. Para indicar que finalizou o carregamento.
                    if (leaderBoardLine.Position + 1 == result.Leaderboard.Count)
                    {
                        //Atualiza a váriavel para verdadeiro indicando que os dados ainda já foram carregados.
                        FacebookAndPlayFabInfo.leaderboardLoaded = true;
                        //Atualiza a váriavel para falso indicando que os dados estão já foram carregados.
                        FacebookAndPlayFabInfo.leaderboardIsLoading = false;
 
                       Debug.Log("Os dados do Leaderboard foram carregados.");
                    }
 
                    Debug.Log("Não foi possível coletar os dados do usuário no Facebook. Erro: " + e.Message);
                }
            }
            else
            {
                //Verifica se está no ultimo registro do Leadeboard. Para indicar que finalizou o carregamento.
                if (leaderBoardLine.Position + 1 == result.Leaderboard.Count)
                {
                    //Atualiza a váriavel para verdadeiro indicando que os dados ainda já foram carregados.
                    FacebookAndPlayFabInfo.leaderboardLoaded = true;
                    //Atualiza a váriavel para falso indicando que os dados estão já foram carregados.
                    FacebookAndPlayFabInfo.leaderboardIsLoading = false;
 
                   Debug.Log("Os dados do Leaderboard foram carregados.");
                }
 
                Debug.Log("Não foi possível coletar os dados do usuário no Facebook.");
            }
        });
    }
}
