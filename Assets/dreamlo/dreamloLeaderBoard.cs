using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class dreamloLeaderBoard : MonoBehaviour
{
    string dreamloWebserviceURL = "http://dreamlo.com/lb/";

    public string privateCode = "";
    public string publicCode = "";

    string highScores = "";

    public struct Score
    {
        public string playerName;
        public float seconds;
        public string shortText;
        public string dateString;
    }

    public void Setup()
    {
        this.highScores = "";
        LoadScores();
    }

    public static dreamloLeaderBoard GetSceneDreamloLeaderboard()
    {
        GameObject go = GameObject.Find("dreamloPrefab");

        if (go == null)
        {
            Debug.LogError("Could not find dreamloPrefab in the scene.");
            return null;
        }
        return go.GetComponent<dreamloLeaderBoard>();
    }

    public static double DateDiffInSeconds(System.DateTime now, System.DateTime olderdate)
    {
        var difference = now.Subtract(olderdate);
        return difference.TotalSeconds;
    }

    System.DateTime _lastRequest = System.DateTime.Now;
    int _requestTotal = 0;

    bool TooManyRequests()
    {
        var now = System.DateTime.Now;

        if (DateDiffInSeconds(now, _lastRequest) <= 2)
        {
            _lastRequest = now;
            _requestTotal++;
            if (_requestTotal > 3)
            {
                Debug.LogError("DREAMLO Too Many Requests. Am I inside an update loop?");
                return true;
            }

        }
        else
        {
            _lastRequest = now;
            _requestTotal = 0;
        }

        return false;
    }

    public void AddScore(string playerName, float _time)
    {
        if (TooManyRequests()) return;

        int _decimale = (int)_time;
        int _mantissa = (int)((_time - _decimale) * Mathf.Pow(10, (_time.ToString().Length - _decimale.ToString().Length)));

        StartCoroutine(AddScoreWithPipe(playerName, _decimale, _mantissa));
    }

    // This function saves a trip to the server. Adds the score and retrieves results in one trip.
    IEnumerator AddScoreWithPipe(string playerName, int _decimale, int _mantissa)
    {
        playerName = Clean(playerName);
        string url = dreamloWebserviceURL + privateCode + "/add-pipe/" + UnityWebRequest.EscapeURL(playerName + "-" + SystemInfo.deviceUniqueIdentifier) + "/" + _decimale.ToString() + "/" + _mantissa.ToString();
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                highScores = webRequest.downloadHandler.text;
            }
        }
    }

    IEnumerator GetScores()
    {
        highScores = "";
        string url = dreamloWebserviceURL + publicCode + "/pipe";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                highScores = webRequest.downloadHandler.text;
            }
        }
    }

    IEnumerator GetSingleScore(string playerName)
    {
        highScores = "";
        string url = dreamloWebserviceURL + publicCode + "/pipe-get/" + UnityWebRequest.EscapeURL(playerName);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
            int page = pages.Length - 1;

            if (webRequest.isNetworkError)
            {
                Debug.Log(pages[page] + ": Error: " + webRequest.error);
            }
            else
            {
                highScores = webRequest.downloadHandler.text;
            }
        }
    }

    public void LoadScores()
    {
        if (TooManyRequests()) return;
        StartCoroutine(GetScores());
    }

    public string[] ToStringArray()
    {
        if (this.highScores == null) return null;
        if (this.highScores == "") return null;

        string[] rows = this.highScores.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        return rows;
    }

    public List<Score> ToListLowToHigh()
    {
        Score[] scoreList = this.ToScoreArray();

        if (scoreList == null) return new List<Score>();

        List<Score> genericList = new List<Score>(scoreList);

        genericList.Sort((x, y) => x.seconds.CompareTo(y.seconds));

        return genericList;
    }

    public List<Score> ToListHighToLow()
    {
        Score[] scoreList = this.ToScoreArray();

        if (scoreList == null) return new List<Score>();

        List<Score> genericList = new List<Score>(scoreList);

        genericList.Sort((x, y) => y.seconds.CompareTo(x.seconds));

        return genericList;
    }

    public Score[] ToScoreArray()
    {
        string[] rows = ToStringArray();
        if (rows == null) return null;

        int rowcount = rows.Length;

        if (rowcount <= 0) return null;

        Score[] scoreList = new Score[rowcount];

        for (int i = 0; i < rowcount; i++)
        {
            string[] values = rows[i].Split(new char[] { '|' }, System.StringSplitOptions.None);

            Score current = new Score();

            current.playerName = values[0].Split(new char[] { '-' }, System.StringSplitOptions.None)[0];
            current.seconds = 0;
            current.shortText = "";
            current.dateString = "";
            if (values.Length > 2) current.seconds = CheckFloat(values[1], values[2]);
            if (values.Length > 3) current.shortText = values[3];
            if (values.Length > 4) current.dateString = values[4];
            scoreList[i] = current;
        }

        return scoreList;
    }

    // Keep pipe and slash out of names

    string Clean(string s)
    {
        s = s.Replace("/", "");
        s = s.Replace("|", "");
        s = s.Replace("-", "");
        return s;

    }

    float CheckFloat(string _decimaleString, string _mantissaString)
    {
        int _decimale = 0;
        int _mantissa = 0;

        int.TryParse(_decimaleString, out _decimale);
        int.TryParse(_mantissaString, out _mantissa);

        float returnValue = _decimale + ((float)_mantissa / Mathf.Pow(10, _mantissaString.Length));

        return returnValue;
    }
}
