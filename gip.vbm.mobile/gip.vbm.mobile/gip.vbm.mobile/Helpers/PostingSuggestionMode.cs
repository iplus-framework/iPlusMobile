// Copyright (c) 2024, gipSoft d.o.o.
// Licensed under the GNU GPLv3 License. See LICENSE file in the project root for full license information.
﻿using gip.mes.facility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gip.vbm.mobile.Helpers
{
    public class PostingSuggestionMode
    {
        public PostingSuggestionMode()
        {

        }

        public PostingSuggestionMode(PostingQuantitySuggestionMode? mode, string validSeqNo, PostingQuantitySuggestionMode? mode2, string validSeqNo2)
        {
            QuantityMode = mode.HasValue ? mode.Value : PostingQuantitySuggestionMode.OrderQuantity;
            QuantityMode2 = mode2.HasValue ? mode2.Value : PostingQuantitySuggestionMode.None;

            if (validSeqNo != null)
            {
                List<Tuple<int, int>> result = MapValidSequenceNo(validSeqNo);
                if (result.Any())
                    ValidSeqNo = result;
            }

            if (validSeqNo != null)
            {
                List<Tuple<int, int>> result = MapValidSequenceNo(validSeqNo);
                if (result.Any())
                    ValidSeqNo = result;
            }

            if (validSeqNo2 != null)
            {
                List<Tuple<int, int>> result = MapValidSequenceNo(validSeqNo2);
                if (result.Any())
                    ValidSeqNo2 = result;
            }
        }

        private List<Tuple<int, int>> MapValidSequenceNo(string seqNo)
        {
            var parts = seqNo.Split(',');
            List<Tuple<int, int>> result = new List<Tuple<int, int>>();

            foreach (var part in parts)
            {
                if (part.Contains("-"))
                {
                    var fromTo = part.Split('-');
                    string from = fromTo.FirstOrDefault();
                    string to = fromTo.LastOrDefault();

                    int fromSeq = 0;
                    if (!string.IsNullOrEmpty(from) && int.TryParse(from, out fromSeq))
                    {
                        int toSeq = 0;
                        if (!string.IsNullOrEmpty(to) && int.TryParse(to, out toSeq))
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

            return result;
        }

        public PostingQuantitySuggestionMode QuantityMode
        {
            get;
            set;
        }

        public PostingQuantitySuggestionMode QuantityMode2
        {
            get;
            set;
        }

        public List<Tuple<int, int>> ValidSeqNo
        {
            get;
            private set;
        }

        public List<Tuple<int, int>> ValidSeqNo2
        {
            get;
            private set;
        }

        public PostingQuantitySuggestionMode GetPostingSuggestionMode(int seqenceNo)
        {
            if (ValidSeqNo != null && ValidSeqNo.FirstOrDefault(c => c.Item1 <= seqenceNo && seqenceNo <= c.Item2) != null)
            {
                return QuantityMode;
            }

            if (ValidSeqNo2 != null && ValidSeqNo2.FirstOrDefault(c => c.Item1 <= seqenceNo && seqenceNo <= c.Item2) != null)
            {
                return QuantityMode2;
            }

            return PostingQuantitySuggestionMode.OrderQuantity;
        }



        public bool IsSuggestionModeValidFor(int seqenceNo)
        {
            if (ValidSeqNo == null)
                return true;

            return ValidSeqNo.FirstOrDefault(c => c.Item1 <= seqenceNo && seqenceNo <= c.Item2) != null;
        }

    }
}
