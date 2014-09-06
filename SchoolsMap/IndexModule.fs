namespace SchoolsMap

open Nancy

type IndexModule() as x =
    inherit NancyModule()
    do x.Get.["/"] <- fun _ -> 
        box x.View.["index"]

    do x.Get.["/schools"] <- fun _ -> 

        "{
            \"Lat\": 53.7996388, 
            \"Lon\": -1.5491220999999768,
            \"Name\": \"Leeds School\",
            \"Type\": \"secondary\",
            \"Private\": true,
            \"Religious\": false,
            \"Rank\": 0.75
         }" :> obj

