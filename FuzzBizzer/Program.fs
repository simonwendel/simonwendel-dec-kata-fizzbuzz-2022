// WikiFuzz FuzzBizzer - a Web-enabled lazy implementation of FizzBuzz
// Copyright (C) 2022  Simon Wendel
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

open System.Diagnostics
open System.IO
open canopy.classic
open canopy.configuration

let openWikiListOfDivisors () =
    url "https://en.wikipedia.org/"
    "#searchInput" << "Table of divisors"
    click "#searchButton"
    click "1 to 100"

let wikiDivisors number =
    fastTextFromCSS $"th:has([title*='%d{number} (number)'])+td"
    |> List.head
    |> fun x -> x.Split ","
    |> List.ofArray
    |> List.map int

let constructPuzzle rules number =
    let rulesMap = Map.ofList rules

    let collectWords =
        wikiDivisors
        >> List.map (fun n -> Map.tryFind n rulesMap)
        >> List.choose id

    match collectWords number with
    | [] -> string number
    | values -> String.concat "" values

type Log =
    { file: StreamWriter
      show: unit -> unit }

let log =
    let filename = "log.txt"
    let file = File.CreateText(filename)

    { file = file
      show =
          fun () ->
              file.Dispose()
              Process.Start(@"notepad.exe", filename) |> ignore }

let run puzzle =
    chromeDir <- System.AppContext.BaseDirectory
    start chrome

    openWikiListOfDivisors ()

    [ 1 .. 100 ]
    |> List.iter (puzzle >> fprintfn log.file "%s")

    log.show ()
    quit chrome

run <| constructPuzzle [ 3, "Fizz"; 5, "Buzz" ]
