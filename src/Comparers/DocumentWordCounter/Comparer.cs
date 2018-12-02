using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using DocumentPlagiarismChecker.Core;

namespace DocumentPlagiarismChecker.Comparers.WordCounter
{
    /// <summary>
    /// The Word Counter Comparer reads a pair of files and counts how many words and how many times appear on each file, and then calculates
    /// how many of those appearences matches between documents. So, two documents with the same amount of the same words can be a copy with
    /// a high level of provability.
    /// </summary>
    /// <typeparam name="Document"></typeparam>
    internal class Comparer: Core.BaseComparer<Document>
    {  
        /// <summary>
        /// Creates a new instance for the comparer.
        /// </summary>
        /// <param name="fileLeftPath">The left side file's path.</param>
        /// <param name="fileRightPath">The right side file's path.</param>
        /// <returns></returns>
        public Comparer(string fileLeftPath, string fileRightPath): base(fileLeftPath, fileRightPath){
        }  
        
        /// <summary>
        /// Counts how many words and how many times appears within each document, and checks the matching percentage.
        /// </summary>
        /// <returns>The matching's results.</returns>
        public override ComparerMatchingScore Run(){
            ComparerMatchingScore cr = new ComparerMatchingScore("Document Word Counter");            
            cr.DetailsCaption = new string[] { "Word", "Count left", "Count right", "Matching" };
            
            Dictionary<string, int[]> counter = new Dictionary<string, int[]>();
            foreach(string word in this.Left.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][0] += Left.WordAppearances[word];
            }

            foreach(string word in this.Right.WordAppearances.Select(x => x.Key)){
                if(!counter.ContainsKey(word)) counter.Add(word, new int[]{0, 0});
                counter[word][1] += Right.WordAppearances[word];
            }

            //Calculate the matching for each individual word.
            float match = 0;
            foreach(string word in counter.Select(x => x.Key)){                
                int left = counter[word][0];
                int right = counter[word][1];                

                if(left == 0 || right == 0) 
                    match = 0;
                else 
                    match = (left < right ? (float)left / (float)right : (float)right / (float)left);

                cr.AddMatch(match);
                cr.DetailsData.Add(new string[]{word, left.ToString(), right.ToString(), string.Format("{0}%", MathF.Round(match, 2))});                
            }                                    
            
            return cr;
        }        
    }   
}