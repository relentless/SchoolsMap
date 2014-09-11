open System
open System.IO
open HttpClient  
open FSharp.Data

type Location = JsonProvider<"data/LookupJsonExample.txt">

let lookup name =
    let searchString = name + ",Yorkshire"
    let jsonResults = (createRequest Get ("https://maps.googleapis.com/maps/api/geocode/json?address=" + searchString + "&key=AIzaSyBY0C0KvGuucsRw7adOrXnsir25tEPAUpQ") |> getResponseBody)
    let results = Location.Parse(jsonResults)
    results.Results

let showGetResults school (locations:Location.Result []) index =
    printfn "%s Lat: %f Lon: %f" school locations.[index].Geometry.Location.Lat locations.[index].Geometry.Location.Lng
    (locations.[index].Geometry.Location.Lat, locations.[index].Geometry.Location.Lng)

[<EntryPoint>]
let main _ = 
    
    printfn "Looking up exact location of schools"

    let dataLines =
        File.ReadAllLines(__SOURCE_DIRECTORY__ + """\data\secondary.csv""").[1..]
        |> Array.append (File.ReadAllLines(__SOURCE_DIRECTORY__ + """\data\primary.csv""").[1..])
        |> Array.map (fun line -> line.Split(','))

    printfn "%i records to process\n" dataLines.Length

    let outputLines =
        dataLines
        |> Array.map (fun schoolInfo ->

            Threading.Thread.Sleep(100) // sleep to avoid breaking 10 request/sec google geocoding api limit

            let name = schoolInfo.[0].Replace("'","")
            let locations = lookup name

            let (lat,lon) = 
                match locations.Length with
                | 0 -> printfn "\n**No locations found for %s**\n" name
                       (0M, 0M)
                | 1 -> showGetResults name locations 0
                | _ -> 
                    let choices = locations |> Array.mapi (fun index location -> sprintf "%i %s\n" index location.FormattedAddress)
                    printfn "\nMore than one location was found for '%s'.  Please enter selection or X for none:\n%s" name (choices |> Array.reduce (+))

                    match Console.ReadKey().KeyChar.ToString() with
                    | "x" -> (0M, 0M)
                    | selectedIndex -> showGetResults name locations (int selectedIndex)

            sprintf "%s,%f,%f,%f,%f,%s,%s,%s,%s" name lat lon (float schoolInfo.[2]) (float (schoolInfo.[3].Replace("%", ""))) schoolInfo.[4] schoolInfo.[5] schoolInfo.[6] schoolInfo.[7])

    let outputIncludingHeader = outputLines |> Array.append [|"name, latitude, longitude, rank, score, private, religious, catholic, type"|]
    
    File.WriteAllLines( """..\..\data\school_locations.csv""", outputIncludingHeader )

    0
