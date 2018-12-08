using System;
using System.Collections.Generic;
using System.Text;

namespace FormsMvvm
{
    public class ApiResponseGourmet
    {
        public Results results { get; set; }

        public class Results
        {
            public Error error { get; set; }
            public string api_version { get; set; }
            public string results_available { get; set; }
            public string results_returned { get; set; }
            public string results_start { get; set; }
            public List<Shop> shop { get; set; }
        }

        public class Error
        {
            public string code { get; set; }
            public string message { get; set; }
        }

        public class Shop
        {
            public string id { get; set; }
            public string name { get; set; }
            public string logo_image { get; set; }
            public string name_kana { get; set; }
            public string address { get; set; }
            public string station_name { get; set; }
            public Photo photo { get; set; }
        }

        public class Photo
        {
            public Pc pc { get; set; }
            public Mobile mobile { get; set; }

            public class Pc
            {
                public string l { get; set; }
                public string m { get; set; }
                public string s { get; set; }
            }

            public class Mobile
            {
                public string l { get; set; }
                public string s { get; set; }
            }
        }
    }
}
