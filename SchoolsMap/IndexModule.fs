namespace SchoolsMap

open Nancy
open System
open System.IO
open Newtonsoft.Json

type SchoolPre = {
    Name: string;
    Lat: decimal;
    Lon: decimal;
    Rank: float;
    Score: float;
    Private: bool;
    Religious: bool;
    Catholic: bool
}

type School = {
    Name: string;
    Lat: decimal;
    Lon: decimal;
    Rank: float;
    Score: float;
    Icon: string
}

type IndexModule() as x =
    inherit NancyModule()

    let getIcon = function
        | { SchoolPre.Rank = rank; SchoolPre.Private = true } when rank > 0.75 -> "1stQuartile_$.png"
        | { SchoolPre.Rank = rank; SchoolPre.Private = true } when rank <= 0.75 -> "2ndQuartile_$.png"
        | { SchoolPre.Rank = rank } when rank > 0.75 -> "1stQuartile.png"
        | { SchoolPre.Rank = rank } when rank <= 0.75 -> "2ndQuartile.png"

    let loadSchoolData() = 
        File.ReadAllLines("""C:\Programming\SchoolsMap\SchoolsMap\data\school_locations.csv""").[1..]
        |> Array.map (fun (line:string) -> line.Split(','))
        |> Array.map (fun school -> {Name = school.[0]; Lat = decimal school.[1]; Lon = decimal school.[2]; Rank = float school.[3]; Score = float school.[4]; Private = Convert.ToBoolean(school.[5]); Religious = Convert.ToBoolean(school.[6]); Catholic = Convert.ToBoolean(school.[7]); })
        |> Array.map (fun school -> {Name = school.Name; Lat = school.Lat; Lon = school.Lon; Rank = school.Rank; Score = school.Score; Icon = school |> getIcon})
        |> Array.toList 


    do x.Get.["/"] <- fun _ -> 
        box x.View.["index"]

    do x.Get.["/schools"] <- fun _ -> 

        let schoolData = loadSchoolData()
        let json = JsonConvert.SerializeObject(schoolData)

        json :> obj
