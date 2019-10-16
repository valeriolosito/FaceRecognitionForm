using Facebook;
using FaceRecognitionForm.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;

namespace FaceRecognitionForm.Service
{

    public class FacebookService
    {
        private string AppId = ConfigurationManager.AppSettings["appID"];
        private Uri _loginUrl;
        public FacebookOAuthResult _FacebookOAuthResult { get; private set; }
        private const string _ExtendedPermissions = "user_hometown, user_birthday";
        private FacebookClient fbClient = new FacebookClient();

        public FacebookService()
        {
            if (string.IsNullOrEmpty(AppId))
                throw new ArgumentNullException("appId");
            var _oauthClient = new FacebookOAuthClient { AppId = AppId };
            IDictionary<string, object> _loginParameters = new Dictionary<string, object>();
            _loginParameters["response_type"] = "token";
            _loginParameters["display"] = "popup";
            if (!string.IsNullOrEmpty(_ExtendedPermissions))
            {
                _loginParameters["scope"] = _ExtendedPermissions;
            }
            _loginUrl = _oauthClient.GetLoginUrl(_loginParameters);
        }

        public string getAbsoluteUri()
        {
            return _loginUrl.AbsoluteUri;
        }

        public string getAccessToken(WebBrowser webBroweser)
        {
            string access_token;
            string url1 = webBroweser.Url.AbsoluteUri;
            string url2 = url1.Substring(url1.IndexOf("access_token") + 13);
            access_token = url2.Substring(0, url2.IndexOf("&"));
            return access_token;
        }
       
        //user data
        public string getUserId(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me");
            return data.id;
        }

        public string getName(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me");
            return data.name;}

        //amici che hanno l'app installata
        public List<Friend> GetFriendList(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic friendsListData = fb.Get("me/friends");
            int count = (int)friendsListData.data.Count;
            List<Friend> friendList = new List<Friend>();
            for (int i = 0; i < count; i++)
            {
                try
                {
                    string name = friendsListData.data[i].name;
                    string id = friendsListData.data[i].id;
                    Friend temp = new Friend(name,id);
                    friendList.Add(temp);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            return friendList;
        }
        public List<Like> GetLikes(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            string userId = getUserId(access_token);
            string path = String.Format("{0}/likes", userId);
            dynamic data = fb.Get(path);
            List<Like> likes = new List<Like>();
            for (int i = 0; i < (int)data.data.Count; i++)
            {
                string name = data.data[i].name;
                string id = data.data[i].id;
                DateTime dateTime = Convert.ToDateTime(data.data[i].created_time);
                Like like = new Like(name, id, dateTime);
                likes.Add(like);
            }
            return likes;
        }

        public List<Movie> GetMovies(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            string userId = getUserId(access_token);
            string path = String.Format("{0}/movies", userId);
            dynamic data = fb.Get(path);
            List<Movie> movies = new List<Movie>();
            for (int i = 0; i < (int)data.data.Count; i++)
            {
                string name = data.data[i].name;
                string id = data.data[i].id;
                DateTime dateTime = Convert.ToDateTime(data.data[i].created_time);
                Movie movie = new Movie(name, id, dateTime);
                movies.Add(movie);
            }
            return movies;
        }

        //data di nascita
        public string GetBirthday(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me?fields=birthday");
            return data.birthday;
        }

        public string GetGender(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me?fields=gender");
            return data.gender;
        }

        public string GetEmail(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me?fields=email");
            return data.email;
        }

        public string GetHometown(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me?fields=hometown");
            return data.hometown;
        }

        public string GetLocation(string access_token)
        {
            FacebookClient fb = new FacebookClient(access_token);
            dynamic data = fb.Get("me?fields=location");
            return data.location;
        }

        public void FacebookOAuth(WebBrowserNavigatedEventArgs e)
        {
            FacebookOAuthResult _oauthResult;
            if (FacebookOAuthResult.TryParse(e.Url, out _oauthResult))
            {
                this._FacebookOAuthResult = _oauthResult;
                try
                {
                    var fb = new FacebookClient(_FacebookOAuthResult.AccessToken);
                    var result = (IDictionary<string, object>)fb.Get("me");
                    var id = (string)result["id"];
                    string profilePictureUrl = string.Format("https://graph.facebook.com/{0}/picture?type={1}", id, "square");
                   
                    Type tresult = result.GetType();
                    var objname = (string)result["name"];
                    var firstName = (string)result["first_name"];
                    var lastName = (string)result["last_name"];
                }
                catch (FacebookApiException ex)
                {
                    throw new FacebookApiException(ex.Message);
                }
            }
            else
            {
                this._FacebookOAuthResult = null;
            }
        }
    }
}
