using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordSearchSolver
{
    class Program
    {
        // when calling this function, supply the path to the csv file with the grid and then the file with the word list
        static void Main(string[] args)
        {
            string fileLocation = @"D:\PersonalProjects\WordSearch";

            WordSearch wordSearch = new WordSearch(fileLocation + "\\TestSearch.csv", fileLocation + "\\wordsToFind.txt");
            List<WordSearch.position> startingPoints = wordSearch.FindStartingPoints();
            List<string> pointsAsStrings = new List<string>();
            pointsAsStrings.Add("(x,y) direction");
            foreach (var position in startingPoints)
            {
                pointsAsStrings.Add("("+position.x + "," + position.y + ")  " + position.direction );
            }
            File.WriteAllLines(fileLocation + "\\startingLocations.txt", pointsAsStrings);
        }

    }
    class WordSearch
    {
        public string[][] board;
        public string[] words;
        int xMax;
        int yMax;
        Dictionary<string, List<WordSearch.position>> letters;
        public WordSearch(string filePath, string wordPath)
        {
            board = GetWordGrid(filePath);

            words = GetWordList(wordPath);

            letters = GetLetterPositions();

            xMax = board.GetLength(0);

            yMax = board[1].GetLength(0);
        }
        public struct position
        {
            public int x;
            public int y;
            public string direction;
        }
        public string[][] GetWordGrid(string path)
        {
            StreamReader sr = new StreamReader(path);
            var lines = new List<string[]>();
            int Row = 0;
            while (!sr.EndOfStream)
            {
                string[] Line = sr.ReadLine().Split(',');
                lines.Add(Line);
                Row++;
                Console.WriteLine(Row);
            }

            var data = lines.ToArray();
            return data;
        }
        public string[] GetWordList(string wordPath)
        {
            string[] lines = System.IO.File.ReadAllLines(wordPath);
            return lines;
        }
        public Dictionary<string, List<position>> GetLetterPositions()
        {
            Dictionary<string, List<position>> letters = new Dictionary<string, List<position>>();
            int x = 0;
            int y = 0;
            foreach (string[] line in board)
            {
                y = 0;
                foreach (string letter in line)
                {
                    if (letters.ContainsKey(letter))
                    {
                        letters[letter].Add(new position { x = x, y = y });
                    }
                    else
                    {
                        List<position> positions = new List<position>();
                        positions.Add(new position { x = x, y = y });
                        letters.Add(letter, positions);
                    }
                    y++;
                }
                x++;
            }
            return letters;
        }
        public List<position> FindStartingPoints()
        {
            List<position> startingPoints = new List<position>();
            foreach (var word in words)
            {
                string found = "NO";
                foreach (position position in letters[word[0].ToString()])
                {
                    found = FindWord(word, position);
                    if (found!="NO")
                    {
                        position newPos = position;
                        newPos.direction = found;
                        startingPoints.Add(newPos);
                        break;
                    }
                }

            }
            return startingPoints;
        }
        internal string FindWord(string word, position position)
        {
            // X is across the top of the grid
            // Y is the side of the grid
            // (0,0) is in the top left hand corner

            // check for word in all directions
            // up left diagonal
            string ULD = GetNextLetter(-1, -1, word.Length, position);
            // going up
            string LEFT = GetNextLetter(0, -1, word.Length, position);
            // up right diagonal
            string LLD = GetNextLetter(1, -1, word.Length, position);
            // going right
            string DOWN = GetNextLetter(1, 0, word.Length, position);
            // lower right diagonal
            string LRD = GetNextLetter(1, 1, word.Length, position);
            // going down
            string RIGHT = GetNextLetter(0, 1, word.Length, position);
            // lower left diagonal
            string URD = GetNextLetter(-1, 1, word.Length, position);
            // going left
            string UP = GetNextLetter(-1, 0, word.Length, position);
            if(ULD==word)
            {
                return "ULD";
            }
            else if (LEFT == word)
            {
                return "LEFT";

            }
            else if (LLD == word)
            {
                return "LLD";

            }
            else if (DOWN == word)
            {
                return "DOWN";

            }
            else if (LRD == word)
            {
                return "LRD";

            }
            else if (RIGHT == word)
            {
                return "RIGHT";

            }
            else if (URD == word)
            {
                return "URD";

            }
            else if (UP == word)
            {
                return "UP";

            }
            return "NO";
        }
        internal string GetNextLetter(int deltaX, int deltaY, int length, position position)
        {
            string nextLetter = board[position.x][position.y];
            if (length > 0)
            {
                position.x += deltaX;
                position.y += deltaY;
            }
            if ((position.x < 0 || position.y < 0 || position.y >= yMax || position.x >= xMax)&&(length>1))
            {
                return "FAIL";
            }
            if (length > 1)
                return nextLetter + GetNextLetter(deltaX, deltaY, length - 1, position);
            return nextLetter;
        }

    }
}
