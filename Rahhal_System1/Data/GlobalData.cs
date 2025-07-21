using Rahhal_System1.DAL;
using Rahhal_System1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Rahhal_System1.Data
{
    public static class GlobalData
    {
        // ✅ القوائم العامة
        public static List<User> UsersList { get; set; }
        public static List<Country> CountriesList { get; set; }
        public static List<City> CitiesList { get; set; }
        public static List<Trip> TripsList { get; set; }
        public static List<CityVisit> CityVisitsList { get; set; }
        public static List<Phrase> PhrasesList { get; set; }

        // ✅ يتم استدعاء هذا المُنشئ الثابت مرة واحدة عند أول استخدام للكلاس
        static GlobalData()
        {
            RefreshAll();
        }

        // ✅ دوال تحديث مستقلة لكل قائمة
        public static void RefreshUsers() => UsersList = UserDAL.GetAllUsers();
        public static void RefreshCountries() => CountriesList = CountryDAL.GetAllCountries();
        public static void RefreshTrips(int userId) => TripsList = TripDAL.GetTripsByUser(userId);
        public static void RefreshCities(int countryId) => CitiesList = CityDAL.GetCitiesByCountry(countryId);
        public static void RefreshCityVisits(int tripId) => CityVisitsList = CityVisitDAL.GetVisitsByTrip(tripId);
        public static void RefreshPhrases(int userId) => PhrasesList = PhraseDAL.GetPhrasesByUser(userId);

        // ✅ لتحديث الكل مرة واحدة عند بدء التطبيق (مع تحديد userId و countryId... إذا لزم)
        public static void RefreshAll()
        {
            RefreshUsers();
            RefreshCountries();

            // ✳️ يُفضّل أن تحدد ID المستخدم أو البلد عند الاستخدام العملي
            TripsList = new List<Trip>();
            CitiesList = new List<City>();
            CityVisitsList = new List<CityVisit>();
            PhrasesList = new List<Phrase>();
        }
    }
}
