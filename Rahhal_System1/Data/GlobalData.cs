using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rahhal_System1.Data
{
    // كلاس ثابت يحتوي على البيانات المشتركة التي تُستخدم في كل أنحاء البرنامج
    public static class GlobalData
    {
        // ✅ قائمة المستخدمين – تُحمّل مرة واحدة وتبقى محفوظة في الذاكرة
        public static List<User> UsersList { get; private set; } = new List<User>();

        // ✅ قائمة البلدان – نفس الشيء، تُحمّل عند بدء البرنامج
        public static List<Country> CountriesList { get; private set; } = new List<Country>();

        // ✅ قائمة المدن – تُحدّث حسب البلد المختار من قبل المستخدم
        public static List<City> CitiesList { get; private set; } = new List<City>();

        // ✅ قائمة الرحلات – تُحدّث حسب المستخدم
        public static List<Trip> TripsList { get; private set; } = new List<Trip>();

        // ✅ قائمة زيارات المدن داخل الرحلة – تُحدّث حسب الرحلة المختارة
        public static List<CityVisit> CityVisitsList { get; private set; } = new List<CityVisit>();

        // ✅ قائمة العبارات (الكلمات) – تُحدّث حسب المستخدم
        public static List<Phrase> PhrasesList { get; private set; } = new List<Phrase>();

        // ✅ هذا هو الـ static constructor – يتنفذ تلقائيًا مرة وحدة فقط عند أول استخدام للكلاس
        static GlobalData()
        {
            RefreshAll(); // تحميل البيانات الأساسية (المستخدمين والبلدان) وتفريغ الباقي
        }

        // ✅ دالة لجلب كل المستخدمين من قاعدة البيانات عند بدء البرنامج
        // ✅ دالة لجلب كل المستخدمين من قاعدة البيانات
        public static void RefreshUsers() => UsersList = UserDAL.GetAllUsers();

        // ✅ دالة لجلب جميع البلدان من قاعدة البيانات
        public static void RefreshCountries() => CountriesList = CountryDAL.GetAllCountries();

        // ✅ دالة لجلب الرحلات الخاصة بمستخدم معين
        public static void RefreshTrips(int userId) => TripsList = TripDAL.GetTripsByUser(userId);

        // ✅ دالة لجلب المدن الخاصة ببلد معين
        public static void RefreshCities(int countryId) => CitiesList = CityDAL.GetCitiesByCountry(countryId);

        // ✅ دالة لجلب زيارات المدن الخاصة برحلة معينة
        public static void RefreshCityVisits(int tripId) => CityVisitsList = CityVisitDAL.GetVisitsByTrip(tripId);

        // ✅ دالة لجلب العبارات الخاصة بمستخدم معين
        public static void RefreshPhrases(int userId) => PhrasesList = PhraseDAL.GetPhrasesByUser(userId);

        // ✅ دالة تقوم بتحديث المستخدمين والبلدان، وتفرغ القوائم الأخرى
        // (اللي تعتمد على اختيارات لاحقة مثل المستخدم أو البلد أو الرحلة)
        public static void RefreshAll()
        {
            RefreshUsers();        // تحميل جميع المستخدمين
            RefreshCountries();    // تحميل جميع البلدان

            // ✳️ تفريغ القوائم التي تعتمد على اختيارات المستخدم لاحقًا
            TripsList.Clear();         // تفريغ الرحلات
            CitiesList.Clear();        // تفريغ المدن
            CityVisitsList.Clear();    // تفريغ زيارات المدن
            PhrasesList.Clear();       // تفريغ العبارات
        }
    }
}
