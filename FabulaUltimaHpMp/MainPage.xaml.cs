
using System.Diagnostics;

namespace FabulaUltimaHpMp
{
    public partial class MainPage : ContentPage
    {
        int criticalHp, maximumHP, currentHP, maximumMP, currentMP;
        List<Button> hpButtons;
        List<Button> mpButtons;
        public MainPage()
        {
            InitializeComponent();
            hpButtons = new List<Button>() { buttonHpm1, buttonHpm2, buttonHpm5, buttonHpm10, buttonHpp1, buttonHpp2, buttonHpp5, buttonHpp10 };
            mpButtons = new List<Button>() { buttonMpm1, buttonMpm2, buttonMpm5, buttonMpm10, buttonMpp1, buttonMpp2, buttonMpp5, buttonMpp10 };
            foreach (Button button in hpButtons) { button.IsEnabled = false; }
            foreach (Button button in mpButtons) { button.IsEnabled = false; }
            hpResetButton.IsVisible = false;
            mpResetButton.IsVisible = false;
        }

        private void MaxHpEntry_Completed(object sender, EventArgs e)
        {
            string input = MaxHpEntry.Text;
            string output = "";
            foreach (char c in input)
            {
                if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0') output = output + c.ToString();
            }
            currentHP = int.Parse(output); maximumHP = currentHP;
            criticalHp = (int)Math.Floor((float)currentHP / 2);
            crisisHp.Text = criticalHp.ToString();
            currentHpCounter.Text = currentHP.ToString();
            MaxHpReminder.Text = currentHP.ToString();
            MaxHpEntry.IsVisible = false;
            MaxHpEntry.IsVisible = false;
            MaxHpEntryLabel.IsVisible = false;
            foreach (Button button in hpButtons) { button.IsEnabled = true; }
            hpResetButton.IsVisible = true;

        }

        private void MaxMpEntry_Completed(object sender, EventArgs e)
        {
            string input = MaxMpEntry.Text;
            string output = "";
            foreach (char c in input)
            {
                if (c == '1' || c == '2' || c == '3' || c == '4' || c == '5' || c == '6' || c == '7' || c == '8' || c == '9' || c == '0') output = output + c.ToString();
            }
            currentMP = int.Parse(output); maximumMP = currentMP;
            currentMpCounter.Text = currentMP.ToString();
            MaxMpReminder.Text = currentMP.ToString();
            MaxMpEntry.IsVisible = false;
            MaxMpEntry.IsVisible = false;
            MaxMpEntryLabel.IsVisible = false;
            foreach (Button button in mpButtons) { button.IsEnabled = true; }
            mpResetButton.IsVisible = true;
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
            else if (currentHP + hp < 0) { hpIncrement += 0-currentHP;  currentHP = 0; }
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

        async void hpResetButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Warning!", "Do you want to reset HP?", "Yes", "No");
            if (answer)
            {
                hpButtons = new List<Button>() { buttonHpm1, buttonHpm2, buttonHpm5, buttonHpm10, buttonHpp1, buttonHpp2, buttonHpp5, buttonHpp10 };
                foreach (Button button in hpButtons) { button.IsEnabled = false; }
                hpResetButton.IsVisible = false;
                MaxHpEntry.IsVisible = true;
                MaxHpEntryLabel.IsVisible = true;
                MaxHpReminder.Text = "";
                currentHpCounter.Text = "";
                crisisHp.Text = "";
            }
        }

        async void mpResetButton_Clicked(object sender, EventArgs e)
        {
            bool answer = await DisplayAlert("Warning!", "Do you want to reset MP?", "Yes", "No");
            if (answer)
            {
                mpButtons = new List<Button>() { buttonMpm1, buttonMpm2, buttonMpm5, buttonMpm10, buttonMpp1, buttonMpp2, buttonMpp5, buttonMpp10 };
                foreach (Button button in mpButtons) { button.IsEnabled = false; }
                mpResetButton.IsVisible = false;
                MaxMpEntry.IsVisible = true;
                MaxMpEntryLabel.IsVisible = true;
                MaxMpReminder.Text = "";
                currentMpCounter.Text = "";
            }
        }

        int hpTicksRemaining = 0;
        int hpIncrement = 0;
        bool hpModVisible = false;
        async void HpModifying()
        {
            if (hpModVisible) return;
            hpModVisible = true;
            hpChange.IsVisible = true;
            while(hpTicksRemaining>5)
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


        int mpTicksRemaining = 0;
        int mpIncrement = 0;
        bool mpModVisible = false;
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

    }

}
