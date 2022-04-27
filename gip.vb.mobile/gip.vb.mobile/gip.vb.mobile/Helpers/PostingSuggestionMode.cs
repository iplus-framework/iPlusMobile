using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gip.vb.mobile.Helpers
{
    public class PostingSuggestionMode
    {
        public PostingSuggestionMode()
        {

        }

        public PostingSuggestionMode(PostingQuantitySuggestionMode mode, string validSeqNo)
        {
            QuantityMode = mode;

            var parts = validSeqNo.Split(',');
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var fromTo = part.Split('-');
                    string from = fromTo.FirstOrDefault();
                    string to = fromTo.LastOrDefault();

                    int fromSeq = 0;
                    if (!string.IsNullOrEmpty(part) && int.TryParse(part, out fromSeq))
                    {
                        int toSeq = 0;
                        if (!string.IsNullOrEmpty(part) && int.TryParse(part, out toSeq))
                        {
                            result.Add(new Tuple<int, int>(fromSeq, toSeq));
                        }
                    }
                }
                else
                {
                    int seq = 0;
                    if (!string.IsNullOrEmpty(part) && int.TryParse(part, out seq))
                    {
                        result.Add(new Tuple<int, int>(seq, seq));
                    }
                }
            }

            if (result.Any())
                ValidSeqNo = result;


        }

        public PostingQuantitySuggestionMode QuantityMode
        {
            get;
            set;
        }

        public List<Tuple<int, int>> ValidSeqNo
        {
            get;
            private set;
        }

        public bool IsSuggestionModeValidFor(int seqenceNo)
        {
            if (ValidSeqNo == null)
                return true;

            return ValidSeqNo.FirstOrDefault(c => c.Item1 >= seqenceNo && seqenceNo <= c.Item2) != null;
        }

    }
}
