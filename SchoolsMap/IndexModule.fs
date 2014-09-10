namespace SchoolsMap

open Nancy
open System.IO
open Newtonsoft.Json

type School = {
    Name: string;
    Lat: decimal;
    Lon: decimal;
    Rank: float
}

type IndexModule() as x =
    inherit NancyModule()

    let loadSchoolData() = 
        File.ReadAllLines("""C:\Programming\SchoolsMap\SchoolsMap\data\school_locations.csv""").[1..]
        |> Array.map (fun (line:string) -> line.Split(','))
        |> Array.map (fun school -> {Name = school.[0]; Lat = decimal school.[1]; Lon = decimal school.[2]; Rank = float school.[3]})
        |> Array.toList 


    do x.Get.["/"] <- fun _ -> 
        box x.View.["index"]

    do x.Get.["/schools"] <- fun _ -> 

        let schoolData = loadSchoolData()
        let json = JsonConvert.SerializeObject(schoolData)

        json :> obj
