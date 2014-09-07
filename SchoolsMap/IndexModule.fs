namespace SchoolsMap

open Nancy
open Newtonsoft.Json

type School = {
    Name: string;
    Lat: decimal;
    Lon: decimal;
    Rank: float
}

type IndexModule() as x =
    inherit NancyModule()
    do x.Get.["/"] <- fun _ -> 
        box x.View.["index"]

    do x.Get.["/schools"] <- fun _ -> 

        let json = 
            JsonConvert.SerializeObject(
                [
                    { Name = "Leeds School";
                      Lat = 53.7996388m;
                      Lon = -1.5491220999999768m;
                      Rank = 0.75 };
                    { Name = "Other School";
                      Lat = 53.787547m;
                      Lon = -1.543452m;
                      Rank = 0.75 }
                 ])

        json :> obj
