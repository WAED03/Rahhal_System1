using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using Rahhal_System1.DAL;
using Rahhal_System1.Models;

namespace Rahhal_System1.Forms
{
    public partial class NewWord : Form
    {
        // متغير لتخزين رقم العبارة عند التعديل، null يعني إضافة جديدة
        private int? editingPhraseID = null;

        // المنشئ الافتراضي يستخدم إضافة جديدة (يمرر null)
        public NewWord() : this(null) { }

        // منشئ للتحرير يأخذ معرف العبارة لتحميلها وتعديلها
        public NewWord(int? phraseID)
        {
            InitializeComponent(); // تهيئة مكونات الفورم
            editingPhraseID = phraseID; // حفظ المعرف للتحرير أو null للإضافة الجديدة
            this.Load += NewWord_Load; // ربط حدث تحميل الفورم
        }

        // عند تحميل الفورم، نحمل بيانات الرحلات فقط، والمدن لاحقًا حسب الرحلة
        private void NewWord_Load(object sender, EventArgs e)
        {
            LoadTrips();   // تحميل الرحلات الخاصة بالمستخدم
            cbTrip.SelectedIndexChanged += cbTrip_SelectedIndexChanged; // ربط حدث تغيير الرحلة

            if (editingPhraseID != null)
            {
                LoadPhraseData((int)editingPhraseID); // تحميل بيانات العبارة للتحرير
            }
        }

        // دالة لتحميل الرحلات وربطها بالـ ComboBox الخاص بالرحلات
        private void LoadTrips()
        {
            var trips = TripDAL.GetTripsByUser(ActivityLogger.CurrentUser.UserID);
            cbTrip.DataSource = trips;
            cbTrip.DisplayMember = "TripName";
            cbTrip.ValueMember = "TripID";

            // تحميل المدن لأول رحلة تلقائيًا عند فتح الفورم
            if (trips.Count > 0)
            {
                LoadCitiesByTrip(trips[0].TripID);
            }
        }


        // عند تغيير الرحلة يتم تحميل المدن الخاصة بها فقط
        private void cbTrip_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbTrip.SelectedValue is int selectedTripId)
            {
                LoadCitiesByTrip(selectedTripId);
            }
        }


        // هذا الدالة لتحميل زيارات المدن المرتبطة برحلة معينة
        private void LoadCitiesByTrip(int tripId)
        {
            var visits = CityVisitDAL.GetVisitsByTrip(tripId);  // فقط الزيارات المرتبطة بالرحلة
            cbCity.DataSource = visits;
            cbCity.DisplayMember = "CityNameForDisplay";  // خاصية نضيفها في CityVisit لعرض اسم المدينة
            cbCity.ValueMember = "VisitID";
        }


        // التحقق من صحة جميع الحقول المطلوبة قبل الحفظ
        private bool ValidateFields()
        {
            bool isValid = true;
            errorProvider1.Clear(); // مسح الأخطاء السابقة

            if (cbTrip.SelectedItem == null)
            {
                errorProvider1.SetError(cbTrip, "Please select a trip");
                isValid = false;
            }

            if (cbCity.SelectedItem == null)
            {
                errorProvider1.SetError(cbCity, "Please select a city");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtOriginal.Text))
            {
                errorProvider1.SetError(txtOriginal, "Please enter the original phrase");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtTranslation.Text))
            {
                errorProvider1.SetError(txtTranslation, "Please enter the translation");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLanguage.Text))
            {
                errorProvider1.SetError(txtLanguage, "Please specify the language");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtNotes.Text))
            {
                errorProvider1.SetError(txtNotes, "Please enter notes");
                isValid = false;
            }

            return isValid;
        }

        // عند الضغط على زر الحفظ
        private void btnSaveWord_Click(object sender, EventArgs e)
        {
            if (!ValidateFields()) return; // إيقاف التنفيذ إذا لم تكن البيانات صحيحة

            // إعداد كائن Phrase من البيانات المدخلة
            var phrase = new Phrase
            {
                VisitID = (int)cbCity.SelectedValue,
                OriginalText = txtOriginal.Text.Trim(),
                Translation = txtTranslation.Text.Trim(),
                Language = txtLanguage.Text.Trim(),
                Notes = txtNotes.Text.Trim()
            };

            bool success;

            // تحديد هل هو إضافة أو تعديل
            if (editingPhraseID == null)
            {
                success = PhraseDAL.AddPhrase(phrase); // إضافة جديدة
            }
            else
            {
                phrase.PhraseID = editingPhraseID.Value;
                success = PhraseDAL.UpdatePhrase(phrase); // تعديل موجود
            }

            if (success)
            {
                string action = editingPhraseID == null ? "Add Phrase" : "Update Phrase";
                using (var con = DbHelper.GetConnection())
                {
                    con.Open();
                    ActivityLogger.Log(con, action, $"Phrase: {phrase.OriginalText} ({phrase.Language})");
                }

                MessageBox.Show("✅ Phrase saved successfully.");
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("❌ Operation failed.");
            }
        }

        // تحميل بيانات عبارة موجودة لغرض التعديل
        private void LoadPhraseData(int phraseID)
        {
            Phrase phrase = PhraseDAL.GetPhraseById(phraseID);
            if (phrase != null)
            {
                Trip trip = TripDAL.GetTripByVisitId(phrase.VisitID);
                if (trip != null)
                {
                    cbTrip.SelectedValue = trip.TripID;      // تعيين الرحلة
                    LoadCitiesByTrip(trip.TripID);            // تحميل المدن الخاصة بالرحلة
                    cbCity.SelectedValue = phrase.VisitID;   // تعيين المدينة (الزيارة)
                }

                txtOriginal.Text = phrase.OriginalText;
                txtTranslation.Text = phrase.Translation;
                txtLanguage.Text = phrase.Language;
                txtNotes.Text = phrase.Notes;
            }
        }
    }
}


