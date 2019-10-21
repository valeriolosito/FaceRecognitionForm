
using DarrenLee.Media;
using Facebook;
using FaceRecognitionForm.Model;
using FaceRecognitionForm.Service;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using DlibDotNet;
using FaceRecognitionForm.Utility;
using IronPython.Modules;
using Microsoft.Scripting.Utils;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http;

namespace FaceRecognitionForm
{
    public partial class MainForm : Form
    {
        FacebookService fbService = new FacebookService();
        LoginService loginService = new LoginService();
        RegisterService registerService = new RegisterService();
        DeleteService deleteService = new DeleteService();
        FeedbackService feedbackService = new FeedbackService();
        RecommandationService recommandationService = new RecommandationService();

        Affectiva affectiva;

        private Camera camera = new Camera();
        private Panel currentPanel;
        private const string IMAGE_NAME = "IMAGE";

        private string mess = String.Empty;

        List<Like> likesList = new List<Like>();
        List<Movie> moviesList = new List<Movie>();
        private string genreAffectiva = String.Empty;

        private bool recommandationFacebook = false;
        private bool recommandationAffectiva = false;
        private bool isAffectivaReady = false;



        private enum RecommandationType
        {
            AFFECTIVA,
            FACEBOOK
        }

        private RecommandationType recommandationType;

        //private int fbCount = 0;

        enum Panels
        {
            PanelHomePage,
            PanelRegister,
            PanelDelete,
            PanelPerson,
            PanelPhoto,
            PanelLogin,
            PanelFacebookLogin,
            PanelUserDataFacebook,
            PanelTableUserDataFacebook
        }
        
        public MainForm()
        {
            InitializeComponent();
            GetInfoCamera();
        }

        private void MyCamera_OnFrameArrived(object source, FrameArrivedEventArgs e)
        {
            Image img = e.GetFrame();
            this.pictureBox_Photo.Image = img;
        }

        private void setCurrentPanel(Panel panel)
        {
            this.currentPanel = panel;
            this.currentPanel.BringToFront();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelHomePage);
        }

        private void btnRegister_HomePage_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelRegister);
            cmbAddressType_Register.SelectedIndex = 0;
        }

        private void btnLogin_HomePage_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelLogin);
            camera.Start();
            camera.OnFrameArrived += CameraLogin_OnFrameArrived;
        }

        private void btnDelete_HomePage_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelDelete);
        }

        private void btnPrevious_Delete_Click(object sender, EventArgs e)
        {
            resetPanelValue(this.panelDelete);
            setCurrentPanel(this.panelHomePage);
        }

        private void btnDeleteUser_Delete_Click(object sender, EventArgs e)
        {
            string taxCode = this.txtTaxCode_Delete.Text;
            if (!Utilities.isValidTaxCode(taxCode))
            {
                MessageBox.Show("Invalid Tax Code!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            else
            {
                if (MessageBox.Show("The user with Tax Code: " + taxCode + " will be permanently deleted, do you want to continue?", "Information",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        int rowsAffected = this.deleteService.RemoveUser(txtTaxCode_Delete.Text);
                        if (rowsAffected == 2) // se è stato rimosso l'utente da Person (1 riga) e la relativa foto da Image (1 riga)
                        {
                            MessageBox.Show("The user with Tax Code: " + taxCode + " has been deleted!", "Information",
                                MessageBoxButtons.OK);
                            resetPanelValue(this.panelDelete);
                            setCurrentPanel(this.panelHomePage);
                        }
                        else
                        {
                            MessageBox.Show("The user with Tax Code: " + taxCode + " doesn't exist!", "Error",
                                MessageBoxButtons.OK);
                        }
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Missing Photo!", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void CameraLogin_OnFrameArrived(object source, FrameArrivedEventArgs e)
        {
            Image img = e.GetFrame();
            this.pictureBoxCamera_Login.Image = img;
        }

        private void btnPrevious_Register_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You will lose all data and photos. Continue?", "Information",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.btnAddPhoto_Register.Enabled = true;
                resetPanelValue(this.currentPanel);
                setCurrentPanel(this.panelHomePage);
            }
        }

        private void btnAddPhoto_Register_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelPhoto);
            camera.Start();
            camera.OnFrameArrived += MyCamera_OnFrameArrived;
        }

        private void btnRegister_Register_Click(object sender, EventArgs e)
        {
            int panelControlsNum = this.panelRegister.Controls.Count;
            bool valid = true;
            //controls textBox value
            foreach (Control c in this.panelRegister.Controls)
            {
                if (c is TextBox)
                {
                    TextBox textBox = (TextBox)c;
                    if (string.IsNullOrEmpty(textBox.Text))
                    {
                        valid = false;
                    }
                }
            }
            if (valid)
            {
                if (validFieldsRegister())
                {
                    if (MessageBox.Show("You will not be able to upload photos, Continue?", "Information",
                        MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        this.btnAddPhoto_Register.Enabled = true;
                        byte[] imgData = null;
                        try
                        {
                            imgData = File.ReadAllBytes(Application.StartupPath + @"\" + IMAGE_NAME + ".jpg");
                            Person user = new Person(
                                this.txtName_Register.Text.ToUpper(),
                                this.txtSurname_Register.Text.ToUpper(),
                                this.txtCF_Register.Text.ToUpper(),
                                this.cmbAddressType_Register.Text.ToUpper() + " " + this.txtAddress_Register.Text.ToUpper() + " " + this.txtAddressNumber_Register.Text.ToUpper(),
                                this.txtCity_Register.Text.ToUpper(),
                                this.txtTelephone_Register.Text.ToUpper(),
                                this.txtProfession_Register.Text.ToUpper(),
                                this.txtEmail_Register.Text
                            );
                            this.registerService.addUser(user, imgData);
                            System.IO.File.Delete(Application.StartupPath + @"\" + IMAGE_NAME + ".jpg");
                            resetPanelValue(currentPanel);
                            setCurrentPanel(this.panelHomePage);
                        }
                        catch (Exception)
                        {
                            MessageBox.Show("Missing Photo!", "Error", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                            setCurrentPanel(this.panelPhoto);
                            camera.Start();
                            camera.OnFrameArrived += MyCamera_OnFrameArrived;
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("You must insert all personal data!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnPreviuos_Login_Click(object sender, EventArgs e)
        {
            camera.Stop();
            setCurrentPanel(this.panelHomePage);
        }

        private void btnFinish_Photo_MouseDown(object sender, MouseEventArgs e)
        {
            setCurrentPanel(this.panelRegister);
            camera.Stop();
        }

        private void btnFacebookLogin_HomePage_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelLoginFacebook);
            ////string mess = this.webBrowserFacebook_Facebook.StatusText;
            this.webBrowserFacebook_Facebook.Visible = (mess == String.Empty);

            string OAuthURL =
                @"https://www.facebook.com/dialog/oauth?client_id=" + ConfigurationManager.AppSettings["appID"] + "&redirect_uri=https://www.facebook.com/connect/login_success.html&response_type=token&scope=email,user_friends";
            this.webBrowserFacebook_Facebook.Navigate(OAuthURL);
        }

        private void webBrowserFacebook_Facebook_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowserFacebook_Facebook.Url.AbsoluteUri.Contains("access_token"))
            {
                mess = "loginFbOK";
                setCurrentPanel(this.panelUserDataFacebook);
                string token = this.fbService.getAccessToken(webBrowserFacebook_Facebook);
                string id = fbService.getUserId(token);

                List<Friend> friendList = fbService.GetFriendList(token);
                for (int i = 0; i < friendList.Count; i++)
                {
                    listView_UserDataFacebook.Items.Add(friendList[i].Name);
                }

                likesList = fbService.GetLikes(token);
                moviesList = fbService.GetMovies(token);
                recommandationFacebook = true;

                string dataNascita = fbService.GetBirthday(token);
                txtNascita_UserDataFacebook.Text = dataNascita;

                string email = fbService.GetEmail(token);
                txtEmail_UserDataFacebook.Text = email;

                string gender = fbService.GetGender(token);
                txtGender_UserDataFacebook.Text = gender;

                string imgURL = string.Format("https://graph.facebook.com/{0}/picture?type=normal", id);
                this.pictureBox_UserDataFacebook.ImageLocation = imgURL;

                //qui recupero le informazioni dell'utente se si era registrato sull'app (tramite email)
                Person personDataReg = registerService.GetInfoUserReg(email);
                if (personDataReg != null)
                {
                    paneltableUserDataFacebook.Visible = true;
                    txtName_UserDataFacebook.Text = personDataReg.Name;
                    txtSurname_UserDataFacebook.Text = personDataReg.Surname;
                    txtTaxcode_UserDataFacebook.Text = personDataReg.Cf;
                    txtAddress_UserDataFacebook.Text = personDataReg.Address;
                    txtCity_UserDataFacebook.Text = personDataReg.City;
                    txtTelephone_UserDataFacebook.Text = personDataReg.Telephone;
                    txtProfession_UserDataFacebook.Text = personDataReg.Profession;
                    enabledControls(this.paneltableUserDataFacebook);
                }
                else
                {
                    paneltableUserDataFacebook.Visible = false;
                }
                enabledControls(this.panelUserDataFacebook);
            }
        }

        private void btnLogin_Login_Click(object sender, EventArgs e)
        {
            try
            {
                //catturo e salvo l'immagine
                camera.Stop();
                if (File.Exists(Application.StartupPath + @"\ImageLogin.jpg"))
                {
                    File.Delete(Application.StartupPath + @"\ImageLogin.jpg");
                }

                string filename = Application.StartupPath + @"\ImageLogin";
                camera.Capture(filename);
                Image tempImage = Utilities.GetCopyImage(filename + ".jpg");

                //verifico se nella foto scattata è presente un solo volto scattata
                int numFaces = UtilityRecognition.getNumberFaces(filename + ".jpg");

                CheckFaces(numFaces);
                if (numFaces != 1)
                {
                    camera.Start();
                }
                else // presente un volto nella foto
                {
                    pictureBoxCamera_Login.Image = tempImage;

                    //provo ad eseguire la login
                    int resultLogin = loginService.Login(pictureBoxCamera_Login.Image);
                    //login effettuata
                    if (resultLogin == 1)
                    {
                        if (MessageBox.Show("Match founded!", "Information",
                                MessageBoxButtons.OK) == DialogResult.OK)
                        {
                            setCurrentPanel(this.panelPerson);
                            Person person = loginService.getLastPersonaFound();
                            this.txtName_Person.Text = person.Name;
                            this.txtSurname_Person.Text = person.Surname;
                            this.txtAddress_Person.Text = person.Address;
                            this.txtCity_Person.Text = person.City;
                            this.txtTelephone_Person.Text = person.Telephone;
                            this.txtProfession_Person.Text = person.Profession;
                            this.pictureBox_Person.Image = tempImage;
                        }
                    }
                    //login fallita: nessun match nel db
                    else if (resultLogin == 0 || resultLogin == 2)
                    {
                        MessageBox.Show("User not registered!", "Error", MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        camera.Start();
                    }

                }
                this.btnAffectiva.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void cmbCameraDevices_Login_SelectedIndexChanged(object sender, EventArgs e)
        {
            camera.ChangeCamera(cmbCameraDevices_Login.SelectedIndex);
        }

        private void cmbCameraResolution_Login_SelectedIndexChanged(object sender, EventArgs e)
        {
            camera.Start(cmbCameraResolution_Login.SelectedIndex);
        }

        private void cmbCameraDevices_Photo_SelectedIndexChanged(object sender, EventArgs e)
        {
            camera.ChangeCamera(cmbCameraDevices_Photo.SelectedIndex);
        }

        private void cmbCameraResolution_Photo_SelectedIndexChanged(object sender, EventArgs e)
        {
            camera.Start(cmbCameraResolution_Photo.SelectedIndex);
        }

        private void GetInfoCamera()
        {
            var cameraDevices = camera.GetCameraSources();
            var cameraResolution = camera.GetSupportedResolutions();
            foreach (var d in cameraDevices)
            {
                cmbCameraDevices_Photo.Items.Add(d);
                cmbCameraDevices_Login.Items.Add(d);
            }
            foreach (var r in cameraResolution)
            {
                cmbCameraResolution_Photo.Items.Add(r);
                cmbCameraResolution_Login.Items.Add(r);
            }
            // nel panel Photo
            cmbCameraDevices_Photo.SelectedIndex = 0;
            cmbCameraResolution_Photo.SelectedIndex = 0;
            //nel panel Login 
            cmbCameraDevices_Login.SelectedIndex = 0;
            cmbCameraResolution_Login.SelectedIndex = 0;
            camera.Stop();
        }

        private void resetPanelValue(Panel panel)
        {
            if (string.Equals(panel.Name, Panels.PanelRegister.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(panel.Name, Panels.PanelTableUserDataFacebook.ToString(), StringComparison.OrdinalIgnoreCase) ||
                string.Equals(panel.Name, Panels.PanelDelete.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                foreach (Control c in panel.Controls)
                {
                    if (c is TextBox)
                    {
                        TextBox textBox = (TextBox)c;
                        textBox.Text = "";
                    }
                }
            }
            else if (string.Equals(panel.Name, Panels.PanelUserDataFacebook.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                listView_UserDataFacebook.Items.Clear();
            }
        }

        private void btnTakePicture_Photo_Click(object sender, EventArgs e)
        {
            string filename = Application.StartupPath + @"\" + IMAGE_NAME;
            camera.Capture(filename);

            int numFaces = UtilityRecognition.getNumberFaces(filename + ".jpg");
            //faccio dei controlli minimali (se è presente almeno un volto)
            CheckFaces(numFaces);
            if (numFaces == 1)
            {
                camera.Stop();
                DialogResult dialogResult = MessageBox.Show("Add this Photo?", "Photo", MessageBoxButtons.OKCancel,
                    MessageBoxIcon.Information);
                if (dialogResult == DialogResult.OK)
                {
                    this.btnAddPhoto_Register.Enabled = false;
                    setCurrentPanel(this.panelRegister);
                }
                else
                {
                    System.IO.File.Delete(Application.StartupPath + @"\" + IMAGE_NAME + ".jpg");
                    camera.Start();
                }
            }
        }

        private void CheckFaces(int numFaces)
        {
            if (numFaces == 0)  // non sono presenti volti nella foto scattata
            {
                MessageBox.Show("No faces found in image!", "Error", MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
                System.IO.File.Delete(Application.StartupPath + @"\" + IMAGE_NAME + ".jpg");
            }
            else if (numFaces >= 2) // sono presenti 2 o più volti nella foto scattata
            {
                MessageBox.Show("Too many faces in image!", "Error", MessageBoxButtons.OK,
                          MessageBoxIcon.Error);
                System.IO.File.Delete(Application.StartupPath + @"\" + IMAGE_NAME + ".jpg");
            }
        }

        private void btnOK_UserDataFacebook_Click(object sender, EventArgs e)
        {
            resetPanelValue(this.panelUserDataFacebook);
            resetPanelValue(this.paneltableUserDataFacebook);
            this.paneltableUserDataFacebook.Visible = false;
            setCurrentPanel(this.panelHomePage);
        }

        private void btnPrevious_LoginFacebook_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelHomePage);
        }

        private void btnHomePage_Person_Click(object sender, EventArgs e)
        {
            affectiva = null;
            this.panelAffectiva.Visible = false;
            this.btnAffectiva.Enabled = true;
            resetPanelValue(this.currentPanel);
            setCurrentPanel(this.panelHomePage);
        }

        private void enabledControls(Panel panel)
        {
            foreach (Control c in panel.Controls)
            {
                if (c is TextBox)
                {
                    TextBox textBox = (TextBox)c;
                    textBox.Enabled = false;

                }
            }
        }

        private void btnAffectiva_Click(object sender, EventArgs e)
        {
            this.btnAffectiva.Enabled = false;
            bool isVisibile = this.panelAffectiva.Visible;
            if (!isVisibile)
            {
                if (affectiva == null)
                {
                    affectiva = new Affectiva();
                    if (affectiva.Engagement != null)
                    {
                        this.panelAffectiva.Visible = true;
                        this.txtEngagement_Affectiva.Text = affectiva.Engagement;
                        this.txtValence_Affectiva.Text = affectiva.Valence;
                        this.txtContempt_Affectiva.Text = affectiva.Contempt;
                        this.txtSurprise_Affectiva.Text = affectiva.Surprise;
                        this.txtAnger_Affectiva.Text = affectiva.Anger;
                        this.txtSadness_Affectiva.Text = affectiva.Sadness;
                        this.txtDisgust_Affectiva.Text = affectiva.Disgust;
                        this.txtFear_Affectiva.Text = affectiva.Fear;
                        this.txtJoy_Affectiva.Text = affectiva.Joy;

                        Dictionary<string, decimal> dict = new Dictionary<string, decimal>();
                        dict.Add("Romance", decimal.Parse(affectiva.Engagement, System.Globalization.NumberStyles.Float));
                        dict.Add("Drama", decimal.Parse(affectiva.Valence, System.Globalization.NumberStyles.Float));
                        dict.Add("History", decimal.Parse(affectiva.Contempt, System.Globalization.NumberStyles.Float));
                        dict.Add("Sci-Fi", decimal.Parse(affectiva.Surprise, System.Globalization.NumberStyles.Float));
                        dict.Add("Fantasy", decimal.Parse(affectiva.Anger, System.Globalization.NumberStyles.Float));
                        dict.Add("Sport", decimal.Parse(affectiva.Sadness, System.Globalization.NumberStyles.Float));
                        dict.Add("Mystery", decimal.Parse(affectiva.Disgust, System.Globalization.NumberStyles.Float));
                        dict.Add("Thriller", decimal.Parse(affectiva.Fear, System.Globalization.NumberStyles.Float));
                        dict.Add("Comedy", decimal.Parse(affectiva.Joy, System.Globalization.NumberStyles.Float));

                        genreAffectiva = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
                        recommandationAffectiva = true;
                    }
                    else
                    {
                        if (MessageBox.Show("Face too far to calculate. Do you want to take a new photo?", "Warning...",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            this.setCurrentPanel(panelLogin);
                            camera.Start();
                            camera.OnFrameArrived += MyCamera_OnFrameArrived;
                        }
                        this.btnAffectiva.Enabled = false;
                        this.panelAffectiva.Visible = false;
                    }
                }
            }
        }

        private bool validFieldsRegister()
        {
            bool result = true;
            //controllo name
            if (!Utilities.containsOnlyLetters(this.txtName_Register.Text))
            {
                MessageBox.Show("Invalid Name!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo surname
            else if (!Utilities.containsOnlyLetters(this.txtSurname_Register.Text))
            {
                MessageBox.Show("Invalid Surname!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo tax code
            else if (!Utilities.isValidTaxCode(this.txtCF_Register.Text))
            {
                MessageBox.Show("Invalid Tax Code!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo l'esistenza tax code
            else if (registerService.getTaxCode(this.txtCF_Register.Text) != null)
            {
                MessageBox.Show("Tax Code Used!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo address type
            else if (this.cmbAddressType_Register.Text == "")
            {
                MessageBox.Show("Address Type Not Selected!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo address
            else if (!Utilities.containsOnlyLetters(this.txtAddress_Register.Text))
            {
                MessageBox.Show("Invalid Address!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo address number
            else if (!Utilities.isValidAddressNumber(this.txtAddressNumber_Register.Text))
            {
                MessageBox.Show("Invalid Address Number!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo city
            else if (!Utilities.containsOnlyLetters(this.txtCity_Register.Text))
            {
                MessageBox.Show("Invalid City!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo telephone
            else if (!Utilities.isValidTelephone(this.txtTelephone_Register.Text))
            {
                MessageBox.Show("Invalid Telephone!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo profession
            else if (!Utilities.containsOnlyLetters(this.txtProfession_Register.Text))
            {
                MessageBox.Show("Invalid Profession!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            //controllo email
            else if (!Utilities.isValidEmail(this.txtEmail_Register.Text))
            {
                MessageBox.Show("Invalid Email!", "Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                result = false;
            }
            return result;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            camera.Stop();
            System.IO.File.Delete(Application.StartupPath + @"\ImageLogin.jpg");
            Application.Exit();
        }

        private void btnFeedbackOK_Recommandation_Click(object sender, EventArgs e)
        {
            this.feedbackService.AddFeedback("yes");
            DisableButton_Recommandation();
        }

        private void btnFeedbackKO_Recommandation_Click(object sender, EventArgs e)
        {
            this.feedbackService.AddFeedback("no");
            DisableButton_Recommandation();
        }

        private void DisableButton_Recommandation()
        {
            MessageBox.Show("Thank you for submitting your Feedback!", "Thank you!", MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            //decommentare
            //btnFeedbackOK_Recommandation.Enabled = false;
            //btnFeedbackKO_Recommandation.Enabled = false;
        }

        private void btnFeedback_HomePage_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelFeedback);
            chartFeedback_Feedback.Series["Series"].Points.Clear();
            var ret = feedbackService.GetFeedback();
            chartFeedback_Feedback.Series["Series"].XValueMember = "Type of Feedback";
            chartFeedback_Feedback.Series["Series"].YValueMembers = "Percentage %";
            //chartFeedback_Feedback.Titles.Add("Feedback Percentage");
            if (ret[2] != 0)
            {
                chartFeedback_Feedback.Series["Series"].Points.AddXY("YES", Math.Truncate(100 * (Decimal.Parse(ret[0].ToString()) / Decimal.Parse(ret[2].ToString()) * 100)) / 100);
                chartFeedback_Feedback.Series["Series"].Points.AddXY("NO", Math.Truncate(100 * (Decimal.Parse(ret[1].ToString()) / Decimal.Parse(ret[2].ToString()) * 100)) / 100);
            }

            chartFeedback_Feedback.Series["Series"].IsValueShownAsLabel = true;


            Title title = new Title();
            title.Font = new Font("Arial", 16, FontStyle.Bold);
            title.Text = "Feedback Result";
            chartFeedback_Feedback.Titles.Clear();
            chartFeedback_Feedback.Titles.Add(title);
        }

        private async void btnRecommandation_HomePage_Click(object sender, EventArgs e)
        {
            if (recommandationFacebook && recommandationAffectiva)
            {
                setCurrentPanel(this.panelRecommendation);

                //se ho già raccomandato un film, non devo cercare un nuovo film ma devo mostrare sempre lo stesso
                if (lblTitle1_Recommandation.Text == "Title Movie" && lblGenre1_Recommandation.Text == "Genre Movie" &&
                    lblActors1_Recommandation.Text == "Actor Movie")
                {
                    List<string> genresList = recommandationService.GetGenres(moviesList);

                    string[] genreSplit = null;
                    Dictionary<string, int> dict = new Dictionary<string, int>();

                    foreach (var singleGenre in genresList)
                    {
                        genreSplit = singleGenre.Split('|');
                        foreach (var genre in genreSplit)
                        {
                            if (dict.ContainsKey(genre))
                            {
                                dict[genre] += 1;
                            }
                            else
                                dict.Add(genre, 1);
                        }
                    }

                    //prende il genere preferito in base ai like ai film su Facebook
                    var favoriteGenre = dict.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;

                    //prende un film casuale dal Dataset con lo stesso genere preferito dall'utente Facebook
                    var recommendedMovieFacebook = this.recommandationService.GetMovie(favoriteGenre);

                    lblTitle1_Recommandation.Text = recommendedMovieFacebook[0];
                    lblGenre1_Recommandation.Text = recommendedMovieFacebook[1];
                    lblActors1_Recommandation.Text = recommendedMovieFacebook[2];
                    this.recommandationType = RecommandationType.FACEBOOK;
                    string linkMovieFacebook = recommendedMovieFacebook[3];


                    setImageMovieFacebook(linkMovieFacebook);
                    //prende un film casuale dal Dataset con lo stesso genere basato su Affectiva
                    var recommendedMovieAffectiva = this.recommandationService.GetMovie(genreAffectiva);

                    lblTitle2_Recommandation.Text = recommendedMovieAffectiva[0];
                    lblGenre2_Recommandation.Text = recommendedMovieAffectiva[1];
                    lblActors2_Recommandation.Text = recommendedMovieAffectiva[2];
                    this.recommandationType = RecommandationType.AFFECTIVA;
                    string linkMovieAffectiva = recommendedMovieAffectiva[3];
                    setImageMovieAffectiva(linkMovieAffectiva);
 


                }

               
                
            }
            else if (recommandationFacebook)
            {
                MessageBox.Show("You must log in and run details image at least once!!", "Thank you!", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
            else if(recommandationAffectiva)
            {
                MessageBox.Show("You must log in via Facebook at least once!!", "Thank you!", MessageBoxButtons.OK,
                   MessageBoxIcon.Error);
            }
            else
            {
                MessageBox.Show("You must log in!", "Thank you!", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

      
        private void btnHome_Feedback_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelHomePage);
        }

        private void btnHome_Recommandation_Click(object sender, EventArgs e)
        {
            setCurrentPanel(this.panelHomePage);
        }

        private void linkGoogle1_Recommandation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.google.com/search?q=" + lblTitle1_Recommandation.Text + "%20film");
        }

        private void linkGoogle2_Recommandation_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Navigate to a URL.
            System.Diagnostics.Process.Start("http://www.google.com/search?q=" + lblTitle2_Recommandation.Text + "%20film");
        }

        private void setImageMovieFacebook(string url)
        {
           
            WebBrowser browserFacebook = new WebBrowser();
            browserFacebook.Navigate(url);
            browserFacebook.ScriptErrorsSuppressed = true;
            browserFacebook.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(browserFacebook_DocumentCompleted);
        }

        private void setImageMovieAffectiva(string url)
        {
            WebBrowser browserAffectiva = new WebBrowser();
            browserAffectiva.Navigate(url);
            browserAffectiva.ScriptErrorsSuppressed = true;
            browserAffectiva.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(browserAffectiva_DocumentCompleted);
        }

        private void browserFacebook_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            var browser = (WebBrowser)sender;
            var client = new WebClient();
            foreach (var img in browser.Document.Images)
            {
                var image = img as HtmlElement;
                var altAttribute = image.GetAttribute("alt");
                if (altAttribute.Contains("Poster"))
                {
                    var srcPath = image.GetAttribute("src").TrimEnd('/');
                    var imagePath = Application.StartupPath + ConfigurationManager.AppSettings["imageFacebookTab"];
                    client.DownloadFile(new Uri(srcPath), imagePath);
                    this.pictureBox_TabFacebook.Image = Utilities.GetCopyImage(imagePath);
                    this.isAffectivaReady = true;
 
                }
            }
        }

        private void browserAffectiva_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            var browser = (WebBrowser)sender;
            var client = new WebClient();
            foreach (var img in browser.Document.Images)
            {
                var image = img as HtmlElement;
                var altAttribute = image.GetAttribute("alt");
                if (altAttribute.Contains("Poster"))
                {
                    var srcPath = image.GetAttribute("src").TrimEnd('/');
                    var imagePath = Application.StartupPath + ConfigurationManager.AppSettings["imageAffectivaTab"];
                    client.DownloadFile(new Uri(srcPath), imagePath);             
                    this.pictureBox_TabAffectiva.Image = Utilities.GetCopyImage(imagePath); ;
                }
            }
        }

        private void btnStampa_Feedback_Click(object sender, EventArgs e)
        {
            PrintPreviewDialog ppd = new PrintPreviewDialog();
            ppd.Document = chartFeedback_Feedback.Printing.PrintDocument;
            ppd.ShowDialog();
        }
    }
}

