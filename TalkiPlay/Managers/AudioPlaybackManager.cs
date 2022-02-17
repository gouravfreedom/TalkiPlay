using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediaManager;
using Splat;
using TalkiPlay.Shared;
using Xamarin.Forms;

namespace TalkiPlay.Managers
{
    public class AudioPlaybackManager
    {
        TaskCompletionSource<bool> _audioPlaybackCompletedSource;
        private static readonly SemaphoreSlim _playbackSemaphoreSlim = new SemaphoreSlim(1, 1);

        private readonly IStorage _storage;
        readonly IConfig _config; 
        readonly IGameMediator _gameMediator;
        
        public AudioPlaybackManager(IGameMediator gameMediator)
        {
            _gameMediator = gameMediator;
            _config = Locator.Current.GetService<IConfig>();
            _storage = Locator.Current.GetService<IStorage>();
        }
        
         #region Audio

        public void SetAudioPlaybackCompletion()
        {
            if (!_audioPlaybackCompletedSource?.Task.IsCompleted ?? false)
            {
                Debug.WriteLine("Audio: MediaItemFinished");
                _audioPlaybackCompletedSource.SetResult(true);
            } 
        }
        
        public async Task PlaySystemSound(bool gameEnded, SoundType type)
        {
            var successAssetId = _gameMediator.CurrentGame.Sounds?.FirstOrDefault(m => m.Type == type)?.Asset.Id;
            if (successAssetId != null)
            {
                await PlayAudio(gameEnded, successAssetId.Value);
            }
        }

        public async Task PlayAudio(bool gameEnded, int assetId, bool waitToFinish = true)
        {
            if (gameEnded)
            {
                return;
            }
            
            Debug.WriteLine($"Audio:{assetId} - Waiting to start");
            
            await _playbackSemaphoreSlim.WaitAsync(2000);
            _playbackSemaphoreSlim.Release();
            
            
            SetAudioPlaybackCompletion();
            
            try
            {

                if (!CrossMediaManager.Current.IsStopped())
                {
                    Debug.WriteLine($"Audio:{assetId} - Stopping");
                    try
                    {
                        await CrossMediaManager.Current.Stop();

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Audio:{assetId} - Stopping: " + ex.Message);
                    }
                }

                //var setting = _assetsSettings.FirstOrDefault(s => s.AssetId == assetId);

                if (waitToFinish)
                {
                    if (_audioPlaybackCompletedSource != null &&
                        !_audioPlaybackCompletedSource.Task.IsCompleted)
                    {
                        _audioPlaybackCompletedSource.TrySetCanceled();
                    }

                    _audioPlaybackCompletedSource = new TaskCompletionSource<bool>();
                }

                var filePath = Path.Combine(_storage.GetRootPath(), $"{assetId}.mp3");

                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                {
                    var info = new FileInfo(filePath);
                    Debug.WriteLine($"Audio:{assetId} - Playing cached file: " + filePath);
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }
                    
                    CrossMediaManager.Current.Play(info).Forget();
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                {
                    filePath =
                        $"{_config.GetAssetDownloadUrl(assetId)}?ApiKey={_config.ApiKey}";
                    Debug.WriteLine($"Audio:{assetId} - Playing url: " + filePath);
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }

                    
                    CrossMediaManager.Current.Play(filePath).Forget();
                    if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                    }
                }

                if (waitToFinish)
                {
                    Debug.WriteLine($"Audio:{assetId} - Waiting to finish");
                    await _audioPlaybackCompletedSource.Task;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlayAudio: " + ex.Message);
            }
            finally
            {
                Debug.WriteLine($"Audio:{assetId} - Completed");
                SetAudioPlaybackCompletion();
                //_playbackSemaphoreSlim.Release();
            }
            
        }
        
        public async Task PlayAudio(string filePath, bool waitToFinish = true)
        {
            Debug.WriteLine($"Audio:{filePath} - Waiting to start");
            await _playbackSemaphoreSlim.WaitAsync(2000);
            _playbackSemaphoreSlim.Release();
            SetAudioPlaybackCompletion();
            
            try
            {
                if (!CrossMediaManager.Current.IsStopped())
                {
                    Debug.WriteLine($"Audio:{filePath} - Stopping");
                    try
                    {
                        await CrossMediaManager.Current.Stop();

                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Audio:{filePath} - Stopping: " + ex.Message);
                    }
                }

                if (!string.IsNullOrEmpty(filePath))
                {
                    if (waitToFinish)
                    {
                        if (_audioPlaybackCompletedSource != null &&
                            !_audioPlaybackCompletedSource.Task.IsCompleted)
                        {
                            _audioPlaybackCompletedSource.TrySetCanceled();
                        }

                        _audioPlaybackCompletedSource = new TaskCompletionSource<bool>();
                    }

                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        if (File.Exists(filePath))
                        {
                            var info = new FileInfo(filePath);
                            Debug.WriteLine($"Audio:{filePath} - Playing cached file: " +
                                            filePath);
                            
                            await CrossMediaManager.Current.Play(info);
                        }
                    }
                    else if (Device.RuntimePlatform == Device.Android)
                    {
                        await Task.Delay(100);
                        await CrossMediaManager.Current.PlayFromResource(filePath);
                        await Task.Delay(100);
                    }

                    if (waitToFinish)
                    {
                        Debug.WriteLine($"Audio:{filePath} - Waiting to finish");
                        await _audioPlaybackCompletedSource.Task;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("PlayAudio: " + ex.Message);
            }
            finally
            {
                Debug.WriteLine($"Audio:{filePath} - Completed");
                SetAudioPlaybackCompletion();
                //_playbackSemaphoreSlim.Release();
            }
          
        }
        
        #endregion
    }
}