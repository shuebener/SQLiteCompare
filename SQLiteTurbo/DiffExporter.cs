using System;
using System.Collections.Generic;
using System.Text;

namespace SQLiteTurbo
{
    /// <summary>
    /// This class is responsible for exporting found changes to a CSV file
    /// </summary>
    public class DiffExporter : AbstractWorker
    {
        #region CONSTRUCTORS
        public DiffExporter(TableChanges changes, string fpath, bool exportUpdates, bool exportAdded, bool exportDeleted)
            : base("Diff Exporter")
        {
            _changes = changes;
            _fpath = fpath;
            _exportAdded = exportAdded;
            _exportDeleted = exportDeleted;
            _exportUpdates = exportUpdates;
        }

        public DiffExporter(List<SchemaComparisonItem> multiChanges, string fpath, bool exportUpdates, bool exportAdded, bool exportDeleted)
            : base("DiffExporter")
        {
            _multiChanges = multiChanges;
            _fpath = fpath;
            _exportAdded = exportAdded;
            _exportDeleted = exportDeleted;
            _exportUpdates = exportUpdates;
        }
        #endregion

        #region Protected Methods
        protected override void DoWork()
        {
            if (_changes != null)
                _changes.ExportToCSV(_fpath, _exportUpdates, _exportAdded, _exportDeleted, ExportHandler);
            else
            {
                // Check how many tables have changes in them
                List<TableChanges> list = new List<TableChanges>();
                foreach (SchemaComparisonItem item in _multiChanges)
                {
                    if (item.TableChanges != null && !item.TableChanges.SameTables)
                        list.Add(item.TableChanges);
                } // foreach
                TableChanges.ExportMultipleToCSV(_fpath, list, _exportUpdates, _exportAdded, _exportDeleted, ExportHandler);
            }  // else
        }

        protected override bool IsDualProgress
        {
            get { return false; }
        }
        #endregion

        #region Private Methods
        private void ExportHandler(long rowsExported, long total, ref bool cancel)
        {
            cancel = WasCancelled;
            if (total == 0)
                return;
            int next = (int)(100.0 * rowsExported / total);
            if (next > _progress)
            {
                _progress = next;
                NotifyPrimaryProgress(false, _progress, rowsExported.ToString() + "/" + total.ToString() + " rows exported", null);
            }
        }
        #endregion


        #region FIELDS
        private int _progress = 0;
        private TableChanges _changes;
        private List<SchemaComparisonItem> _multiChanges;
        private bool _exportUpdates;
        private bool _exportAdded;
        private bool _exportDeleted;
        private string _fpath;
        #endregion
    }
}
