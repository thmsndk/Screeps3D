using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.Screeps3D
{
    public class ClientUpdateDetection : MonoBehaviour
    {
        [SerializeField] private GameObject content;
        [SerializeField] private TMP_Text updateLabel;

        private void Start()
        {
            // Do update check
            StartCoroutine(DetectVersion());
        }

        private IEnumerator DetectVersion()
        {
            string owner = "thmsndk";
            string repo = "Screeps3D";

            var www = UnityWebRequest.Get($"https://api.github.com/repos/{owner}/{repo}/releases");

            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                var responseText = www.downloadHandler.text;
                var response = JsonUtility.FromJson<Rootobject>("{\"releases\":" + responseText + "}"); // Unity only supports parsing objects

                var currentVersion = Application.version;

                var latestRelease = response.releases.FirstOrDefault();

                if (latestRelease.tag_name != currentVersion)
                {
                    updateLabel.text = $"You are running v{currentVersion} {Environment.NewLine} but <link=\"{latestRelease.html_url}\"><color=#3F51B5>v{latestRelease.tag_name}</color></link> has been released";
                    content.SetActive(true);
                }
                else
                {
                    content.SetActive(false);
                }
            }
        }

        [Serializable]
        public class Rootobject
        {
            public Release[] releases;
        }

        [Serializable]
        public class Release
        {
            public string url;
            public string html_url;
            public string assets_url;
            public string upload_url;
            public string tarball_url;
            public string zipball_url;
            public int id;
            public string node_id;
            public string tag_name;
            public string target_commitish;
            public string name;
            public string body;
            public bool draft;
            public bool prerelease;
            public DateTime created_at;
            public DateTime published_at;
            public Author author;
            public Asset[] assets;
        }

        [Serializable]
        public class Author
        {
            public string login;
            public int id;
            public string node_id;
            public string avatar_url;
            public string gravatar_id;
            public string url;
            public string html_url;
            public string followers_url;
            public string following_url;
            public string gists_url;
            public string starred_url;
            public string subscriptions_url;
            public string organizations_url;
            public string repos_url;
            public string events_url;
            public string received_events_url;
            public string type;
            public bool site_admin;
        }
        [Serializable]
        public class Asset
        {
            public string url;
            public string browser_download_url;
            public int id;
            public string node_id;
            public string name;
            public string label;
            public string state;
            public string content_type;
            public int size;
            public int download_count;
            public DateTime created_at;
            public DateTime updated_at;
            public Uploader uploader;
        }
        [Serializable]
        public class Uploader
        {
            public string login;
            public int id;
            public string node_id;
            public string avatar_url;
            public string gravatar_id;
            public string url;
            public string html_url;
            public string followers_url;
            public string following_url;
            public string gists_url;
            public string starred_url;
            public string subscriptions_url;
            public string organizations_url;
            public string repos_url;
            public string events_url;
            public string received_events_url;
            public string type;
            public bool site_admin;
        }

    }
}
