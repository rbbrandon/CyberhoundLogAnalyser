using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace CyberhoundLogAnalyser
{
    public partial class Form1 : Form
    {
        private DataGridViewRow _CurrentRow;

        public Form1()
        {
            InitializeComponent();
        }

        private void processButton_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();

            string dataInRegex = @"In=(\d+)";
            string dataOutRegex = @"Out=(\d+)";
            string urlRegex = @"URL=(.+)";
            
            foreach (string line in logTextBox.Lines)
            {
                if (line == "Done." || line == "")
                    continue;

                try
                {

                    int dataIn = Int32.Parse(Regex.Match(line, dataInRegex).Groups[1].Value);
                    int dataOut = Int32.Parse(Regex.Match(line, dataOutRegex).Groups[1].Value);
                    int totalData = dataIn + dataOut;

                    string uri = Regex.Match(line, urlRegex).Groups[1].Value.ToString();
                    if (!uri.Contains("http"))
                        uri = "http://" + uri;
                    string domain = new Uri(uri).Host;

                    bool found = false;

                    foreach (DataGridViewRow row in dataGridView.Rows)
                    {
                        if (row.Cells[0].Value != null && row.Cells[0].Value.ToString() == domain)
                        {
                            found = true;

                            row.Cells[1].Value = (int)row.Cells[1].Value + dataIn;
                            row.Cells[2].Value = (int)row.Cells[2].Value + dataOut;
                            row.Cells[3].Value = (int)row.Cells[3].Value + totalData;

                            break;
                        }
                    }

                    if (!found)
                    {
                        int index = dataGridView.Rows.Add();
                        dataGridView.Rows[index].Cells[0].Value = domain;
                        dataGridView.Rows[index].Cells[1].Value = dataIn;
                        dataGridView.Rows[index].Cells[2].Value = dataOut;
                        dataGridView.Rows[index].Cells[3].Value = totalData;
                    }
                }
                catch { }
            }

            dataGridView.Sort(dataGridView.Columns[3], ListSortDirection.Descending);
            dataGridView.ClearSelection();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_CurrentRow != null)
            {
                Clipboard.SetText(_CurrentRow.Cells[0].Value.ToString());
            }
        }

        private void dataGridView_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                _CurrentRow = dataGridView.Rows[e.RowIndex];
            else
                _CurrentRow = null;
        }

        private void dataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int rowIndex = dataGridView.HitTest(e.X, e.Y).RowIndex;

            if (rowIndex >= 0)
            {
                
            }
        }

        private void dataGridView_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int rowSelected = e.RowIndex;
                if (e.RowIndex != -1)
                {
                    dataGridView.Rows[rowSelected].Selected = true;
                }
            }
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            var cms = sender as ContextMenuStrip;
            var mousepos = Control.MousePosition;
            if (cms != null)
            {
                var rel_mousePos = cms.PointToClient(mousepos);
                if (cms.ClientRectangle.Contains(rel_mousePos))
                {
                    //the mouse pos is on the menu ... 
                    //looks like the mouse was used to open it
                    var dgv_rel_mousePos = dataGridView.PointToClient(mousepos);
                    var hti = dataGridView.HitTest(dgv_rel_mousePos.X, dgv_rel_mousePos.Y);
                    if (hti.RowIndex == -1)
                    {
                        // no row ...
                        e.Cancel = true;
                    }
                }
                else
                {
                    //looks like the menu was opened without the mouse ...
                    //we could cancel the menu, or perhaps use the currently selected cell as reference...
                    e.Cancel = true;
                }
            }
        }
    }
}
