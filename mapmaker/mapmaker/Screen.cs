using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace mapmaker
{
    [Serializable]
    class Screen
    {
        private int mScreen;
        private List<Row> mRows = new List<Row>();
        
        public Screen(int screenCount)
        {
            mScreen = screenCount;
            InitialiseRows();
        }

        public void InitialiseRows()
        {
            for (int i = 0; i < 10; i++)
            {
                mRows.Add(new Row());
            }
        }

        public void addRow(Row toAdd)
        {
            mRows.Add(toAdd);
        }

        [JsonIgnoreAttribute]
        public int UndergroudScreen
        {
            get { return mScreen; }
        }

        public List<Row> Rows
        {
            get { return mRows; }
            set { mRows = value; }
        }
    }
}
