// WikiFuzz FuzzBizzer - a Lazy implementation of FizzBuzz
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
open canopy.classic
open canopy.configuration

chromeDir <- System.AppContext.BaseDirectory
start chrome

let openWikiListOfDivisors () =
    url "https://en.wikipedia.org/"
    "#searchInput" << "Table of divisors"
    click "#searchButton"
    click "1 to 100"

let wikiDivisorsFor number =
    fastTextFromCSS $"th:has([title*='%d{number} (number)'])+td"
    |> List.head
    |> fun x -> x.Split ","
    |> List.ofArray
    |> List.map int

let constructPuzzle rules number =
    let rulesMap = Map.ofList rules
    let words number = 
        wikiDivisorsFor number 
        |> List.map (fun n -> Map.tryFind n rulesMap) 
        |> List.choose id

    match (words number) with
    | [] -> string number
    | values -> values |> String.concat ""

let run puzzle =
    openWikiListOfDivisors ()
    let filename = "fizzbuzz.txt"
    use file = System.IO.File.CreateText(filename)
    [1..100] |> List.iter (puzzle >> fprintfn file "%s")
    Process.Start(@"notepad.exe", filename) |> ignore

let fizzBuzz = constructPuzzle [3, "Fizz"; 5, "Buzz"]
run fizzBuzz
quit chrome
