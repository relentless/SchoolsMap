namespace LeedsSchoolsMap

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
    Catholic: bool;
    Type: string
}

type School = {
    Name: string;
    Lat: decimal;
    Lon: decimal;
    Rank: float;
    Score: float;
    Type: string;
    Icon: string
}

type IndexModule() as x =
    inherit NancyModule()

    let sixth = (1.0/6.0)

    let rankPart = function
        | rank when rank >= sixth*5.0 -> "1"
        | rank when rank >= sixth*4.0 -> "2"
        | rank when rank >= sixth*3.0 -> "3"
        | rank when rank >= sixth*2.0 -> "4"
        | rank when rank >= sixth*1.0 -> "5"
        | rank when rank >= 0.0 -> "6"
        | _ -> "x" // for schools with an unknown rank

    let typePart = function
        | "Secondary" -> "s"
        | "Primary" -> "p"
        | x -> failwith "Unrecognised school type: " + x

    let trueFalsePart = function
        | true -> "t"
        | false -> "f"

    let religiousPart _ = "f"

    let getIcon (school:SchoolPre) = 
        sprintf "%s_%s_%s_%s_%s.png"
            (school.Type |> typePart)
            (school.Rank |> rankPart)
            (school.Private |> trueFalsePart)
            (school.Religious |> trueFalsePart)
            (school.Catholic |> trueFalsePart)

    let loadSchoolData() = 
        File.ReadAllLines(__SOURCE_DIRECTORY__ + """\data\school_locations.csv""").[1..]
        |> Array.map (fun (line:string) -> line.Split(','))
        |> Array.map (fun school -> {Name = school.[0]; Lat = decimal school.[1]; Lon = decimal school.[2]; Rank = float school.[3]; Score = float school.[4]; Private = Convert.ToBoolean(school.[5]); Religious = Convert.ToBoolean(school.[6]); Catholic = Convert.ToBoolean(school.[7]); Type = school.[8] })
        |> Array.map (fun school -> {Name = school.Name; Lat = school.Lat; Lon = school.Lon; Rank = school.Rank; Score = school.Score; Type = school.Type; Icon = school |> getIcon})
        |> Array.toList 

    let schoolData = loadSchoolData()
    let schoolsJson = JsonConvert.SerializeObject(schoolData)

    do x.Get.["/"] <- fun _ -> 
        box x.View.["index"]

    do x.Get.["/schools"] <- fun _ -> 
        schoolsJson :> obj
