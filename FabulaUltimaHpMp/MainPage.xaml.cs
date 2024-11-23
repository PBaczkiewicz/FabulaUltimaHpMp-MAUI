using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using Color = Microsoft.Maui.Graphics.Color;
using Newtonsoft.Json;
using static Android.Provider.ContactsContract;

namespace FabulaUltimaHpMp
{
    public partial class MainPage : ContentPage
    {
        #region Variables
        int criticalHp, maximumHP, currentHP, maximumMP, currentMP, baseDexDice, dexDice, baseInsDice, insDice, baseMigDice, migDice, baseWilDice, wilDice;
        string editName;
        int editDexDice, editInsDice, editMigDice, editWilDice, editHpCounter = 0, editMpCounter = 0;
        bool editDex, editIns, editMig, editWil;

        int hpTicksRemaining = 0;
        int hpIncrement = 0;
        bool hpModVisible = false;

        int mpTicksRemaining = 0;
        int mpIncrement = 0;
        bool mpModVisible = false;
        #endregion
        #region Lists
        List<bool> readyChecks = null;
        List<Button> hpButtons = null;
        List<Button> mpButtons = null;
        List<Button> dexDiceButtons = null;
        List<Button> insDiceButtons = null;
        List<Button> migDiceButtons = null;
        List<Button> wilDiceButtons = null;
        List<Profile> saveProfiles = null;
        #endregion
        #region Colors
        string whiteColor = "#FFFFFFFF";
        string grayColor = "#808080";
        string greenColor = "#FF1EFF00";
        string orangeColor = "#FFFF7800";
        string redColor = "#FFFF0000";
        #endregion
        public MainPage()
        {
            InitializeComponent();
            InitiateStartUp();
            LoadEditPage();
        }
        async void InitiateStartUp()
        {
            saveProfiles = await LoadProfilesFromJson();

            if (saveProfiles.Count > 0) { LoadProfiles(); }
            hpButtons = new List<Button>() { buttonHpm1, buttonHpm2, buttonHpm5, buttonHpm10, buttonHpp1, buttonHpp2, buttonHpp5, buttonHpp10 };
            mpButtons = new List<Button>() { buttonMpm1, buttonMpm2, buttonMpm5, buttonMpm10, buttonMpp1, buttonMpp2, buttonMpp5, buttonMpp10 };
            dexDiceButtons = new List<Button>() { dexD12, dexD10, dexD8, dexD6 };
            insDiceButtons = new List<Button>() { insD12, insD10, insD8, insD6 };
            migDiceButtons = new List<Button>() { migD12, migD10, migD8, migD6 };
            wilDiceButtons = new List<Button>() { wilD12, wilD10, wilD8, wilD6 };
            readyChecks = new List<bool>() { editDex, editIns, editMig, editWil };
            //foreach (Button button in hpButtons) { button.IsEnabled = false; }
            //foreach (Button button in mpButtons) { button.IsEnabled = false; }
            //hpResetButton.IsVisible = false;
            //mpResetButton.IsVisible = false;
            CheckDices();

            editLayout.IsVisible = true;
            mainLayout.IsVisible = false;
        }



        #region Plus/minus HP buttons
        private void buttonHpp1_Clicked(object sender, EventArgs e) { ModifyHP(1); }
        private void buttonHpp2_Clicked(object sender, EventArgs e) { ModifyHP(2); }
        private void buttonHpp5_Clicked(object sender, EventArgs e) { ModifyHP(5); }
        private void buttonHpp10_Clicked(object sender, EventArgs e) { ModifyHP(10); }
        private void buttonHpm1_Clicked(object sender, EventArgs e) { ModifyHP(-1); }
        private void buttonHpm2_Clicked(object sender, EventArgs e) { ModifyHP(-2); }
        private void buttonHpm5_Clicked(object sender, EventArgs e) { ModifyHP(-5); }
        private void buttonHpm10_Clicked(object sender, EventArgs e) { ModifyHP(-10); }
        #endregion
        private void ModifyHP(int hp)
        {
            if (hpTicksRemaining <= 5) hpIncrement = 0;

            if (currentHP + hp >= maximumHP) { hpIncrement += maximumHP - currentHP; currentHP = maximumHP; }
            else if (currentHP + hp < 0) { hpIncrement += 0 - currentHP; currentHP = 0; }
            else { currentHP += hp; hpIncrement += hp; }
            currentHpCounter.Text = currentHP.ToString();
            if (currentHP <= criticalHp) { currentHpCounter.Text += "!"; }

            hpTicksRemaining = 40;
            hpChange.Text = "";
            if (hpIncrement > 0) hpChange.Text += "+";
            hpChange.Text += hpIncrement.ToString();

            HpModifying();
        }
        #region Plus/minus MP buttons
        private void buttonMpp1_Clicked(object sender, EventArgs e) { ModifyMP(1); }
        private void buttonMpp2_Clicked(object sender, EventArgs e) { ModifyMP(2); }
        private void buttonMpp5_Clicked(object sender, EventArgs e) { ModifyMP(5); }
        private void buttonMpp10_Clicked(object sender, EventArgs e) { ModifyMP(10); }
        private void buttonMpm1_Clicked(object sender, EventArgs e) { ModifyMP(-1); }
        private void buttonMpm2_Clicked(object sender, EventArgs e) { ModifyMP(-2); }
        private void buttonMpm5_Clicked(object sender, EventArgs e) { ModifyMP(-5); }
        private void buttonMpm10_Clicked(object sender, EventArgs e) { ModifyMP(-10); }
        #endregion
        private void ModifyMP(int mp)
        {
            if (mpTicksRemaining <= 5) mpIncrement = 0;

            if (currentMP + mp > maximumMP) { mpIncrement += maximumMP - currentMP; currentMP = maximumMP; }
            else if (currentMP + mp < 0) { mpIncrement += 0 - currentMP; currentMP = 0; }
            else { currentMP += mp; mpIncrement += mp; }
            currentMpCounter.Text = currentMP.ToString();

            mpTicksRemaining = 40;
            mpChange.Text = "";
            if (mpIncrement > 0) mpChange.Text += "+";
            mpChange.Text += mpIncrement.ToString();

            MpModifying();
        }
        #region Status buttons
        bool slowed = false;
        private void slowButton_Clicked(object sender, EventArgs e)
        {
            if (slowed)
            {
                slowButton.BackgroundColor = Color.FromArgb(grayColor);
                slowed = false;
            }
            else
            {
                slowButton.BackgroundColor = Color.FromArgb(whiteColor);
                slowed = true;
            }
            CheckDices();
        }
        bool dazed = false;
        private void dazeButton_Clicked(object sender, EventArgs e)
        {
            if (dazed)
            {
                dazeButton.BackgroundColor = Color.FromArgb(grayColor);
                dazed = false;
            }
            else
            {
                dazeButton.BackgroundColor = Color.FromArgb(whiteColor);
                dazed = true;
            }
            CheckDices();
        }
        bool weaken = false;
        private void weakButton_Clicked(object sender, EventArgs e)
        {
            if (weaken)
            {
                weakButton.BackgroundColor = Color.FromArgb(grayColor);
                weaken = false;
            }
            else
            {
                weakButton.BackgroundColor = Color.FromArgb(whiteColor);
                weaken = true;
            }
            CheckDices();
        }
        bool shaken = false;
        private void shakenButton_Clicked(object sender, EventArgs e)
        {
            if (shaken)
            {
                shakenButton.BackgroundColor = Color.FromArgb(grayColor);
                shaken = false;
            }
            else
            {
                shakenButton.BackgroundColor = Color.FromArgb(whiteColor);
                shaken = true;
            }
            CheckDices();
        }
        bool enraged = false;
        private void enrageButton_Clicked(object sender, EventArgs e)
        {
            if (enraged)
            {
                enrageButton.BackgroundColor = Color.FromArgb(grayColor);
                enraged = false;
            }
            else
            {
                enrageButton.BackgroundColor = Color.FromArgb(whiteColor);
                enraged = true;
            }
            CheckDices();
        }
        bool poisoned = false;
        private void poisonButton_Clicked(object sender, EventArgs e)
        {
            if (poisoned)
            {
                poisonButton.BackgroundColor = Color.FromArgb(grayColor);
                poisoned = false;
            }
            else
            {
                poisonButton.BackgroundColor = Color.FromArgb(whiteColor);
                poisoned = true;
            }
            CheckDices();
        }

        #endregion
        #region EditPage
        void LoadEditPage()
        {
            editDex = false;
            editIns = false;
            editMig = false;
            editWil = false;
            editName = "";
            CheckReadiness();
        }
        private void ProfileNameEntry_Completed(object sender, EventArgs e)
        {
            editName = ProfileNameEntry.Text;
            CheckReadiness();

        }
        private void MaxHpEntry_Completed(object sender, EventArgs e)
        {
            string input = "";

            input += MaxHpEntry.Text;
            string output = "";
            foreach (char c in input)
            {
                if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0') output = output + c.ToString();
            }
            if (output == "")
            {
                MaxHpEntry.TextColor = Color.FromArgb(redColor);
                return;
            }
            MaxHpEntry.TextColor = Color.FromArgb(greenColor);
            editHpCounter = int.Parse(output);
            CheckReadiness();

        }
        private void MaxMpEntry_Completed(object sender, EventArgs e)
        {
            string input = "";
            input += MaxMpEntry.Text;
            string output = "";
            foreach (char c in input)
            {
                if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0') output = output + c.ToString();
            }
            if (output == "")
            {
                MaxMpEntry.TextColor = Color.FromArgb(redColor);
                return;
            }
            MaxMpEntry.TextColor = Color.FromArgb(greenColor);
            editMpCounter = int.Parse(output);
            CheckReadiness();
        }

        #region Dex dices
        private void dexD12_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in dexDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            dexD12.BackgroundColor = Color.FromArgb(greenColor);
            editDexDice = 3;
            editDex = true;
            CheckReadiness();
        }

        private void dexD10_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in dexDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            dexD10.BackgroundColor = Color.FromArgb(greenColor);
            editDexDice = 2;
            editDex = true;
            CheckReadiness();
        }

        private void dexD8_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in dexDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            dexD8.BackgroundColor = Color.FromArgb(greenColor);
            editDexDice = 1;
            editDex = true;
            CheckReadiness();
        }

        private void dexD6_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in dexDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            dexD6.BackgroundColor = Color.FromArgb(greenColor);
            editDexDice = 0;
            editDex = true;
            CheckReadiness();
        }


        #endregion
        #region Ins dices
        private void insD12_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in insDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            insD12.BackgroundColor = Color.FromArgb(greenColor);
            editInsDice = 3;
            editIns = true;
            CheckReadiness();
        }

        private void insD10_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in insDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            insD10.BackgroundColor = Color.FromArgb(greenColor);
            editInsDice = 2;
            editIns = true;
            CheckReadiness();
        }
        private void insD8_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in insDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            insD8.BackgroundColor = Color.FromArgb(greenColor);
            editInsDice = 1;
            editIns = true;
            CheckReadiness();
        }

        private void insD6_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in insDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            insD6.BackgroundColor = Color.FromArgb(greenColor);
            editInsDice = 0;
            editIns = true;
            CheckReadiness();
        }
        #endregion
        #region Mig dices
        private void migD12_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in migDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            migD12.BackgroundColor = Color.FromArgb(greenColor);
            editMigDice = 3;
            editMig = true;
            CheckReadiness();
        }



        private void migD10_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in migDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            migD10.BackgroundColor = Color.FromArgb(greenColor);
            editMigDice = 2;
            editMig = true;
            CheckReadiness();
        }

        private void migD8_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in migDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            migD8.BackgroundColor = Color.FromArgb(greenColor);
            editMigDice = 1;
            editMig = true;
            CheckReadiness();
        }

        private void migD6_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in migDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            migD6.BackgroundColor = Color.FromArgb(greenColor);
            editMigDice = 0;
            editMig = true;
            CheckReadiness();
        }
        #endregion
        #region Wil dices
        private void wilD12_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in wilDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            wilD12.BackgroundColor = Color.FromArgb(greenColor);
            editWilDice = 3;
            editWil = true;
            CheckReadiness();
        }

        private void wilD10_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in wilDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            wilD10.BackgroundColor = Color.FromArgb(greenColor);
            editWilDice = 2;
            editWil = true;
            CheckReadiness();
        }

        private void wilD8_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in wilDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            wilD8.BackgroundColor = Color.FromArgb(greenColor);
            editWilDice = 1;
            editWil = true;
            CheckReadiness();
        }

        private void wilD6_Clicked(object sender, EventArgs e)
        {
            foreach (Button button in wilDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);
            }
            wilD6.BackgroundColor = Color.FromArgb(greenColor);
            editWilDice = 0;
            editWil = true;
            CheckReadiness();
        }
        #endregion
        void CheckReadiness()
        {
            CheckDices();
            finishEditButton.IsEnabled = false;
            saveProfile.IsEnabled = false;
            if (!editDex) return;
            if (!editIns) return;
            if (!editMig) return;
            if (!editWil) return;
            if (MaxHpEntry.Text != null && MaxHpEntry.Text != "")
            {
                editHpCounter = int.Parse(MaxHpEntry.Text);
            }
            if (editHpCounter < 1) return;
            if (MaxMpEntry.Text != null && MaxMpEntry.Text != "")
            {
                editMpCounter = int.Parse(MaxMpEntry.Text);
            }
            if (editMpCounter < 1) return;
            finishEditButton.IsEnabled = true;
            if (ProfileNameEntry.Text != null && ProfileNameEntry.Text != "")
            {
                editName = ProfileNameEntry.Text;
            }
            

            if (editName == "") return;
            
            saveProfile.IsEnabled = true;
        }
        private void finishEditButton_Clicked(object sender, EventArgs e)
        {
            editLayout.IsVisible = false;
            mainLayout.IsVisible = true;
            baseDexDice = editDexDice;
            baseInsDice = editInsDice;
            baseMigDice = editMigDice;
            baseWilDice = editWilDice;
            maximumHP = editHpCounter;
            maximumMP = editMpCounter;
            currentHP = maximumHP;
            currentMP = maximumMP;
            currentHpCounter.IsVisible = true; currentHpCounter.Text = currentHP.ToString(); MaxHpReminder.Text = currentHP.ToString();
            currentMpCounter.IsVisible = true; currentMpCounter.Text = currentMP.ToString(); MaxMpReminder.Text = currentMP.ToString();
            MaxHpReminder.IsVisible = true;
            MaxMpReminder.IsVisible = true;

            CheckDices();
            CalculateCrisis();
        }
        #endregion

        void CheckDices()
        {
            dexDice = baseDexDice;
            if (enraged) dexDice--; if (slowed) dexDice--; if (dexDice < 0) dexDice = 0;
            insDice = baseInsDice;
            if (enraged) insDice--; if (dazed) insDice--; if (insDice < 0) insDice = 0;
            migDice = baseMigDice;
            if (poisoned) migDice--; if (weaken) migDice--; if (migDice < 0) migDice = 0;
            wilDice = baseWilDice;
            if (poisoned) wilDice--; if (shaken) wilDice--; if (wilDice < 0) wilDice = 0;


            if (enraged && slowed) dexDiceButton.Background = Color.FromArgb(redColor);
            else if (enraged || slowed) dexDiceButton.Background = Color.FromArgb(orangeColor);
            else dexDiceButton.Background = Color.FromArgb(greenColor);

            if (enraged && dazed) insDiceButton.Background = Color.FromArgb(redColor);
            else if (enraged || dazed) insDiceButton.Background = Color.FromArgb(orangeColor);
            else insDiceButton.Background = Color.FromArgb(greenColor);

            if (poisoned && weaken) migDiceButton.Background = Color.FromArgb(redColor);
            else if (poisoned || weaken) migDiceButton.Background = Color.FromArgb(orangeColor);
            else migDiceButton.Background = Color.FromArgb(greenColor);

            if (poisoned && shaken) wilDiceButton.Background = Color.FromArgb(redColor);
            else if (poisoned || shaken) wilDiceButton.Background = Color.FromArgb(orangeColor);
            else wilDiceButton.Background = Color.FromArgb(greenColor);




            dexDiceButton.Text = "D" + (6 + dexDice * 2).ToString();
            insDiceButton.Text = "D" + (6 + insDice * 2).ToString();
            migDiceButton.Text = "D" + (6 + migDice * 2).ToString();
            wilDiceButton.Text = "D" + (6 + wilDice * 2).ToString();
        }
        async void HpModifying()
        {
            if (hpModVisible) return;
            hpModVisible = true;
            hpChange.IsVisible = true;
            while (hpTicksRemaining > 5)
            {
                if (hpTicksRemaining >= 15) hpChange.FontSize = 15;
                else hpChange.FontSize = hpTicksRemaining;
                await Task.Delay(100);
                hpTicksRemaining--;
            }
            hpIncrement = 0;
            hpModVisible = false;
            hpChange.IsVisible = false;

        }
        async void MpModifying()
        {
            if (mpModVisible) return;
            mpModVisible = true;
            mpChange.IsVisible = true;
            while (mpTicksRemaining > 5)
            {
                if (mpTicksRemaining >= 15) mpChange.FontSize = 15;
                else mpChange.FontSize = mpTicksRemaining;
                await Task.Delay(100);
                mpTicksRemaining--;
            }
            mpIncrement = 0;
            mpModVisible = false;
            mpChange.IsVisible = false;
        }
        void CalculateCrisis()
        {
            criticalHp = (int)Math.Floor((float)maximumHP / 2);
            crisisHp.Text = criticalHp.ToString();
        }


        private async void saveProfile_Clicked(object sender, EventArgs e)
        {
            Profile profile = new Profile(editName, editDexDice, editInsDice, editMigDice, editWilDice, editHpCounter, editMpCounter);
            foreach (Profile profileCheck in saveProfiles)
            {
                if (profileCheck.profileName == profile.profileName)
                {
                    bool overwrite = await DisplayAlert("Warning!", "You already have a profile with this name!\nDo you want to overwrite this profile?", "Yes", "No");
                    if (overwrite)
                    {
                        profileCheck.profileName = profile.profileName;
                        //profileCheck.profileName=
                    }
                    else
                        return;
                }
            }
            saveProfiles.Add(profile);
            await SaveProfilesToJson(saveProfiles);
            await DisplayAlert("Warning!", "Profile saved!", "OK");
            UpdateProfileList();
        }

        private void ToEditButton_Clicked(object sender, EventArgs e)
        {
            GoToEdit();
        }

        async void GoToEdit()
        {
            bool overwrite = await DisplayAlert("Warning!", "Do you want to go to edit?", "Yes", "No");
            if (overwrite)
            {
                mainLayout.IsVisible = false;
                editLayout.IsVisible = true;
            }

        }


        #region Save&Load
        public async Task SaveProfilesToJson(List<Profile> profiles)
        {
            string jsonString = JsonConvert.SerializeObject(profiles);
            string fileName = "profiles.json";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, fileName);
            await File.WriteAllTextAsync(filePath, jsonString);
        }
        public async Task<List<Profile>> LoadProfilesFromJson()
        {
            string fileName = "profiles.json";
            string filePath = Path.Combine(Microsoft.Maui.Storage.FileSystem.AppDataDirectory, fileName);

            // Check if the file exists
            if (File.Exists(filePath))
            {
                // Read the JSON string from the file
                string jsonString = await File.ReadAllTextAsync(filePath);
                if (jsonString == null || jsonString == "null") return new List<Profile>();
                List<Profile> profiles = JsonConvert.DeserializeObject<List<Profile>>(jsonString);
                return profiles;
            }

            return new List<Profile>(); // Return an empty list if the file doesn't exist
        }
        private void profilePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (profilePicker.SelectedIndex == -1) { return; }
            Profile profile = saveProfiles[profilePicker.SelectedIndex];
            ProfileNameEntry.Text = profile.profileName;
            MaxHpEntry.Text = profile.maxHp.ToString();
            MaxMpEntry.Text = profile.maxMp.ToString();
            #region Dice loading
            foreach (Button button in dexDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);

            }
            if (profile.baseDexDice == 0) dexD6.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseDexDice == 1) dexD8.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseDexDice == 2) dexD10.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseDexDice == 3) dexD12.BackgroundColor = Color.FromArgb(greenColor);

            foreach (Button button in insDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);

            }
            if (profile.baseInsDice == 0) insD6.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseInsDice == 1) insD8.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseInsDice == 2) insD10.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseInsDice == 3) insD12.BackgroundColor = Color.FromArgb(greenColor);

            foreach (Button button in migDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);

            }
            if (profile.baseMigDice == 0) migD6.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseMigDice == 1) migD8.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseMigDice == 2) migD10.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseMigDice == 3) migD12.BackgroundColor = Color.FromArgb(greenColor);

            foreach (Button button in wilDiceButtons)
            {
                button.BackgroundColor = Color.FromArgb(grayColor);

            }
            if (profile.baseWilDice == 0) wilD6.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseWilDice == 1) wilD8.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseWilDice == 2) wilD10.BackgroundColor = Color.FromArgb(greenColor);
            else if (profile.baseWilDice == 3) wilD12.BackgroundColor = Color.FromArgb(greenColor);
            #endregion

            if (MaxMpEntry.Text == "")
            {
                MaxMpEntry.TextColor = Color.FromArgb(redColor);
                return;
            }
            MaxMpEntry.TextColor = Color.FromArgb(greenColor);
            if (MaxHpEntry.Text == "")
            {
                MaxHpEntry.TextColor = Color.FromArgb(redColor);
                return;
            }
            MaxHpEntry.TextColor = Color.FromArgb(greenColor);
            editHpCounter = int.Parse(MaxHpEntry.Text);
            editMpCounter = int.Parse(MaxMpEntry.Text);
            editName = ProfileNameEntry.Text;
            editDex = true;
            editIns = true;
            editMig = true;
            editWil = true;
            CheckReadiness();

            CheckDices();
        }
        void LoadProfiles()
        {
            //profilePicker.ItemsSource = saveProfiles;
            foreach (Profile profile in saveProfiles)
            {
                profilePicker.Items.Add(profile.profileName);
            }

        }

        private void deleteButton_Clicked(object sender, EventArgs e)
        {
            DeleteProfile();
        }
        async void DeleteProfile()
        {
            if (profilePicker.SelectedItem != null)
            {
                bool overwrite = await DisplayAlert("Warning!", "Deleting profile!\nDo you want to delete this profile?", "Yes", "No");
                if (overwrite)
                {
                    Profile remove = new Profile();
                    foreach (Profile profile in saveProfiles)
                    {
                        if (profilePicker.SelectedItem as string == profile.profileName)
                        {
                            remove = profile;
                        }
                    }
                    if (remove.profileName != "")
                        saveProfiles.Remove(remove);
                    UpdateProfileList();
                }
            }
        }

        async void UpdateProfileList()
        {
            profilePicker.SelectedIndex = -1;
            profilePicker.Items.Clear();
            if (saveProfiles.Count > 0)
            {
                foreach (Profile profile in saveProfiles)
                    profilePicker.Items.Add(profile.profileName);
            }
            await SaveProfilesToJson(saveProfiles);
            ProfileNameEntry.Text = "";
            MaxHpEntry.Text = "";
            MaxMpEntry.Text = "";
        }

        #endregion
    }
}