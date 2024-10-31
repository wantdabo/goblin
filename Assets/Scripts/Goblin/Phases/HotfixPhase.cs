using Goblin.Common.FSM;
using Goblin.Sys.Initialize.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YooAsset;

namespace Goblin.Phases
{
    /// <summary>
    /// 资源热更新阶段状态
    /// </summary>
    public enum HotfixPhaseState
    {
        /// <summary>
        /// 未知状态
        /// </summary>
        None,
        /// <summary>
        /// 版本验证
        /// </summary>
        VerifyVer,
        /// <summary>
        /// 检查更新
        /// </summary>
        CheckUpdate,
        /// <summary>
        /// 下载补丁
        /// </summary>
        Download,
        /// <summary>
        /// 完成
        /// </summary>
        Done,
    }

    /// <summary>
    /// 资源热更新阶段
    /// </summary>
    public class HotfixPhase : State
    {
        protected override List<Type> passes => new() { typeof(LoginPhase) };

        /// <summary>
        /// 就绪
        /// </summary>
        public bool finished { get; private set; }

        /// <summary>
        /// 状态
        /// </summary>
        public HotfixPhaseState state { get; private set; } = HotfixPhaseState.None;

        /// <summary>
        /// 需要下载文件数量
        /// </summary>
        public long totalDownloadCount { get; private set; }
        /// <summary>
        /// 当前下载文件数量
        /// </summary>
        public long currentDownloadCount { get; private set; }
        /// <summary>
        /// 需要下载文件的数据大小（字节）
        /// </summary>
        public long totalDownloadBytes { get; private set; }
        /// <summary>
        /// 当前下载文件的数据大小（字节）
        /// </summary>
        public long currentDownloadBytes { get; private set; }

        public override bool OnValid()
        {
            var resp = engine.phase.GetPhase<ResPhase>();

            return false == finished && resp.finished;
        }

        public async override void OnEnter()
        {
            base.OnEnter();
            var package = YooAssets.GetPackage("Package");

            // 校验本地与 CDN 的资源版本文件
            state = HotfixPhaseState.VerifyVer;
            var versionOperation = package.UpdatePackageVersionAsync();
            await versionOperation.Task;
            if (versionOperation.Status == EOperationStatus.Failed) return;

            // 检查需要更新的资源文件
            state = HotfixPhaseState.CheckUpdate;
            var updateOperation = package.UpdatePackageManifestAsync(versionOperation.PackageVersion, false);
            await updateOperation.Task;
            if (updateOperation.Status == EOperationStatus.Failed) return;

            // 创建资源下载器，检查是否有资源需要更新下载
            var downloader = package.CreateResourceDownloader(20, 5);
            if (0 == downloader.TotalDownloadCount)
            {
                finished = true;
                return;
            }

            // 开始更新下载最新的资源文件
            state = HotfixPhaseState.Download;
            downloader.BeginDownload();
            while (true)
            {
                totalDownloadCount = downloader.TotalDownloadCount;
                currentDownloadCount = downloader.CurrentDownloadCount;
                totalDownloadBytes = downloader.TotalDownloadBytes;
                currentDownloadBytes = downloader.CurrentDownloadBytes;
                await Task.Delay(50);
                if (EOperationStatus.Succeed == downloader.Status) break;
            }

            // 资源更新下载完成，保存最新的资源版本号
            state = HotfixPhaseState.Done;
            finished = true;
            updateOperation.SavePackageVersion();
        }
    }
}
