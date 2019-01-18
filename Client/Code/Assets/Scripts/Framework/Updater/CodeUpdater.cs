using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace AosBaseFramework
{
    public sealed class CodeUpdater : UpdaterBase
    {
        private string mVerstionFile = FileHelper.CodeVersionFileCrc.ToString();
        protected override string VersionFile
        {
            get
            {
                return mVerstionFile;
            }
        }

        public override int Progress
        {
            get
            {
                return 100;
            }
        }

        public CodeUpdater(UpdaterManager updaterMgr, UpdaterManager.EUpdaterType eUpdaterType) : 
            base(updaterMgr, eUpdaterType)
        {
        }

        protected override void DownloadDescribeFileAsync()
        {
            base.DownloadDescribeFileAsync();

            BeginHandle?.Invoke(this);
        }

        protected override void LoadDescribe(byte[] data, EPosType ePosType)
        {
            base.LoadDescribe(data, ePosType);

            UpdaterMgr.CodeData = data;
            FinishHandle?.Invoke(this);
        }
    }
}
