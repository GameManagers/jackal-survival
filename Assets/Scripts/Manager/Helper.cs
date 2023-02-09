using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using WE.Bullet;
using WE.Effect;
using WE.Game;
using WE.Manager;
using WE.Pooling;
using WE.SkillEnemy;
using WE.Unit;

namespace WE.Support
{
    public static class Helper
    {
        public static void Despawn(GameObject obj)
        {
            PoolingObject plobj = obj.GetComponent<PoolingObject>();
            if (plobj != null)
                Despawn(plobj);
            else ObjectPooler.Instance.DestroyGameObject(obj);
        }
        public static PoolingObject Spawn(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            return _obj;
        }
        public static T Spawn<T>(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform) where T : PoolingObject
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            return _obj as T;
        }
        public static Enemy SpawnEmptyEnemy(PoolingObject refPrefabs, Vector3 pos)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(EnemySpawner.Instance.transform);
            _obj.transform.position = pos;
            _obj.transform.rotation = Quaternion.identity;
            Enemy e = _obj as Enemy;
            EnemySpawner.Instance.listActiveEnemy.Add(e);
            return e;
        }
        public static Enemy SpawnEmptyEnemy(PoolingObject refPrefabs)
        {
            return SpawnEmptyEnemy(refPrefabs, GetRandomSpawnPos());
        }
        public static T SpawnEmptyEnemy<T>(PoolingObject refPrefabs, Vector3 pos) where T : Enemy
        {
            Enemy e = SpawnEmptyEnemy(refPrefabs, pos);
            return e as T;
        }
        public static T SpawnEmptyEnemy<T>(PoolingObject refPrefabs) where T : Enemy
        {
            return SpawnEmptyEnemy<T>(refPrefabs, GetRandomSpawnPos());
        }
        public static T SpawnEnemy<T>(PoolingObject refPrefabs) where T : Enemy
        {
            return SpawnEnemy<T>(refPrefabs, GetRandomSpawnPos());
        }
        public static T SpawnEnemy<T>(PoolingObject refPrefabs, Vector3 pos) where T : Enemy
        {
            Enemy e = SpawnEmptyEnemy(refPrefabs, pos);
            e.Init();
            return e as T;
        }
        public static Enemy SpawnEnemy(PoolingObject refPrefabs, Vector3 pos)
        {
            Enemy e = SpawnEmptyEnemy(refPrefabs, pos);
            e.Init();
            return e;
        }
        public static Enemy SpawnEnemy(PoolingObject refPrefabs)
        {
            return SpawnEnemy(refPrefabs, GetRandomSpawnPos());
        }
        public static PlayerBullet SpawnBullet(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            return _obj as PlayerBullet;
        }
        public static T SpawnBullet<T>(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform) where T : PlayerBullet
        {
            return SpawnBullet(refPrefabs, position, rotation, parentTransform) as T;
        }
        public static EnemyBullet SpawnEnemyBullet(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            return _obj as EnemyBullet;
        }
        public static T SpawnEnemyBullet<T>(PoolingObject refPrefabs, Vector3 position, Quaternion rotation, Transform parentTransform) where T : EnemyBullet
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = rotation;
            return _obj as T;
        }
        public static AnimationEffect SpawnEffect(PoolingObject refPrefabs, Vector3 position, Transform parentTransform, float fxScale = 1)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = Quaternion.identity;
            _obj.transform.localScale = Vector3.one * fxScale;
            return _obj as AnimationEffect;
        }
        public static T SpawnEffect<T>(PoolingObject refPrefabs, Vector3 position, Transform parentTransform) where T : AnimationEffect
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.SetParent(parentTransform);
            _obj.transform.position = position;
            _obj.transform.rotation = Quaternion.identity;
            return _obj as T;
        }
        public static ItemInGame SpawnItemIngame(Vector3 pos, int value, EItemInGame _type)
        {
            ItemInGame item = Spawn<ItemInGame>(ObjectPooler.Instance.ingameItemPrefabs, pos, Quaternion.identity, null);
            item.type = _type;
            item.value = value;
            item.SetSprite(ObjectPooler.Instance.GetSpriteItem(_type), ObjectPooler.Instance.GetSortingOder(item.type));
            ObjectPooler.Instance.listItemInGames.Add(item);
            item.CheckType();
            if (_type == EItemInGame.Boss_Chest)
            {
                BossChestDirection b = Spawn<BossChestDirection>(ObjectPooler.Instance.bossChestDirection, ObjectPooler.Instance.transform.position, Quaternion.identity, null);
                b.InitItem(item);
            }
            return item;
        }
        public static ItemInGame SpawnExp(Vector3 pos, int value)
        {
            ItemInGame item = SpawnItemIngame(pos, value, EItemInGame.Exp);
            item.SetSprite(ObjectPooler.Instance.GetSpriteExp(value), ObjectPooler.Instance.GetSortingOder(item.type));
            ObjectPooler.Instance.listExp.Add(item);
            return item;
        }
        public static void OnEnemyHit(float damage, Vector3 pos, bool isCrit, bool isHeal = false)
        {
            TextDamage txt = Spawn<TextDamage>(ObjectPooler.Instance.textDamagePrefabs, pos, Quaternion.identity, ObjectPooler.Instance.textImpactParent
                );
            txt.transform.localScale = Vector3.one;
            txt.InitText(damage, isCrit, isHeal);
        }
        public static AudioSourcePoolingObj SpawnAudioSource(PoolingObject refPrefabs)
        {
            PoolingObject _obj = ObjectPooler.Instance.Spawn(refPrefabs);
            _obj.transform.position = Camera.main.transform.position;
            _obj.transform.SetParent(Camera.main.transform);
            return _obj as AudioSourcePoolingObj;
        }
        public static void Despawn(PoolingObject obj)
        {
            ObjectPooler.Instance.Despawn(obj);
        }
        public static float ParseFloat(string data)
        {
            if (data.Contains(","))
                data = data.Replace(",", ".");
            float tmpFloat = float.Parse(data, NumberStyles.Any, CultureInfo.InvariantCulture);
            return tmpFloat;
        }
        public static Dictionary<string, string> GetSubStats(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                Dictionary<string, string> dictStats = str.Split(new string[] { "}{" }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim('{', '}')).Select(s => s.Split(new string[] { ":" }, StringSplitOptions.RemoveEmptyEntries)).ToDictionary(s => s[0], s => s[1]);
                return dictStats;
            }
            else
                return new Dictionary<string, string>();
        }
        public static bool TryToEnum<T>(this string value, out T _type) where T : struct
        {
            if (Enum.TryParse<T>(value, out _type))
            {
                return true;
            }
            return false;
        }
#if UNITY_EDITOR
        public static IEnumerator IELoadData(string urlData, System.Action<string> actionComplete, bool showAlert = false)
        {
            var www = new WWW(urlData);
            float time = 0;
            //TextAsset fileCsvLevel = null;
            while (!www.isDone)
            {
                time += 0.001f;
                if (time > 10000)
                {
                    yield return null;
                    Debug.Log("Downloading...");
                    time = 0;
                }
            }
            if (!string.IsNullOrEmpty(www.error))
            {
                UnityEditor.EditorUtility.DisplayDialog("Notice", "Load CSV Fail", "OK");
                yield break;
            }
            yield return null;
            actionComplete?.Invoke(www.text);
            yield return null;
            UnityEditor.AssetDatabase.SaveAssets();
            if (showAlert)
                UnityEditor.EditorUtility.DisplayDialog("Notice", "Load Data Success", "OK");
            else
                Debug.Log("<color=yellow>Download Data Complete</color>");
        }
#endif
        public static T LoadFromPath<T>(string path) where T : UnityEngine.Object
        {
            return Resources.Load(path, typeof(T)) as T;
        }
        public static AnimationEffect PlayFx(string fxId, Vector3 pos, Transform parent = null)
        {
            AnimationEffect fx = Spawn<AnimationEffect>(DataManager.Instance.dataEffect.dictEffect[fxId], pos, Quaternion.identity, parent);
            return fx;
        }
        public static float getAngle(Vector3 dir)
        {
            return getAngle(dir.x, dir.z);
        }

        public static float getAngle(float x, float y)
        {
            float getAngle_angle = 90f - Mathf.Atan2(y, x) * Mathf.Rad2Deg;
            getAngle_angle = (getAngle_angle + 360f) % 360f;
            return GetFloat2(getAngle_angle);
        }
        public static float Get360Angle(float angle)
        {
            while (angle < 0f)
            {
                angle += 360f;
            }
            while (360f < angle)
            {
                angle -= 360f;
            }
            return angle;
        }
        public static float Get2DAngle(Vector3 dir)
        {
            return Get360Angle(90 - getAngle(dir.x, dir.y));
        }
        public static float Get180Anlge(float angle)
        {
            while (angle > 180)
            {
                angle -= 360;
            } while (angle < -180)
            {
                angle += 360;
            }
            return angle;
        }
        public static float GetFloat2(float f)
        {
            return (float)((int)(f * 100f)) / 100f;
        }
        public static Vector3 GetDirectionFromAngle(float angle)
        {
            float horizontal = Cos(angle);
            float vertical = Sin(angle);
            Vector3 dir = new Vector3(horizontal, vertical, 0);
            return dir;
        }
        public static Vector3 GetPositionFromAngle(Vector3 startPos, float angle, float distance)
        {
            float horizontal = Cos(angle);
            float vertical = Sin(angle);
            Vector3 dir = new Vector3(horizontal, vertical, 0);
            Vector3 pos = startPos + dir * distance;
            return pos;
        }

        public static string ConvertTimer(int second)
        {
            if (second < 0)
                return string.Empty;
            TimeSpan timer = TimeSpan.FromSeconds(second);
            if (timer.TotalHours > 1)
                return timer.ToString(@"hh\:mm");
            return timer.ToString(@"mm\:ss");
        }

        public static string ConvertTimerLongTime(double second)
        {
            if (second < 0)
                return string.Empty;
            TimeSpan timer = TimeSpan.FromSeconds(second);

            if (timer.TotalDays > 1)
                return Math.Round(second / 86400).ToString() + "d " + Math.Round((second % 86400) / 3600).ToString() + "h";
            if (timer.TotalHours > 1)
                return Math.Round(second / 3600).ToString() + "h " + Math.Round((second % 3600) / 60).ToString() + "m";
            return Math.Round(second / 60).ToString() + "m " + Math.Round(second % 60).ToString() + "s";
        }
        public static string GetPercent(float minVal, float currentValue, float totalvalue)
        {
            float percent = currentValue / totalvalue * 100f;
            if (percent < minVal)
            {
                percent = minVal;
            }
            return string.Format("{0:0.#####}%", percent);

        }
        public static float Sin(float angle)
        {
            return Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public static float Cos(float angle)
        {
            return Mathf.Cos(angle * Mathf.Deg2Rad);
        }
        public static string GetTranslation(string key)
        {
            string message = I2.Loc.LocalizationManager.GetTranslation(key);
            if (string.IsNullOrEmpty(message))
                message = key;
            return message;

        }
        public static string TimeToString(TimeSpan t)
        {
            return string.Format(GetTranslation("lb_time_to_string"), t.Minutes, t.Seconds);
        }

        public static Vector3 ClampToScreenSpawnPos(Vector3 pos)
        {

            return pos;
        }
        public static void CastEnemyBullet(Vector3 pos, float radius, LayerMask layerCast)
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(pos, radius, layerCast);
            if (hit != null)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    EnemyBullet b = hit[i].GetComponent<EnemyBullet>();
                    if (b != null)
                        b.ImpactObstacle();
                }

            }
        }
        public static Enemy[] CastDamage(Vector3 pos, float radius, LayerMask layerCast, float damage, float force, Transform target = null, AnimationEffect _fxHit = null, float _fxScale = 1, bool shocked = false, AudioClip _Sfx = null)
        {
            Collider2D[] hit = Physics2D.OverlapCircleAll(pos, radius, layerCast);
            List<Enemy> liste = new List<Enemy>();
            if (_Sfx != null)
                SoundManager.Instance.PlaySoundFx(_Sfx);
            if (hit != null)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    Enemy e = hit[i].GetComponent<Enemy>();
                    if (e != null)
                    {
                        liste.Add(e);
                        e.TakeDamage(damage, force, target, shocked);
                        if (_fxHit != null)
                        {
                            SpawnEffect(_fxHit, e.transform.position, null, _fxScale);
                        }
                    }
                    ChestInGame chest = hit[i].GetComponent<ChestInGame>();
                    if (chest != null)
                        chest.Hit();
                }
            }
            return liste.ToArray();
        }
        public static Enemy[] CastDamage(Vector3 pos, Vector2 size, float angle, LayerMask layerCast, float damage, float force, Transform target = null, AnimationEffect _fxHit = null, float _fxScale = 1, bool shocked = false, AudioClip _Sfx = null)
        {
            Collider2D[] hit = Physics2D.OverlapBoxAll(pos, size, angle, layerCast);
            List<Enemy> liste = new List<Enemy>();
            if (_Sfx != null)
                SoundManager.Instance.PlaySoundFx(_Sfx);
            if (hit != null)
            {
                for (int i = 0; i < hit.Length; i++)
                {
                    Enemy e = hit[i].GetComponent<Enemy>();
                    if (e !=null)
                    {
                        liste.Add(e);
                        e.TakeDamage(damage, force, target, shocked);
                        if (_fxHit != null)
                        {
                            SpawnEffect(_fxHit, e.transform.position, null, _fxScale);
                        }
                    }
                    ChestInGame chest = hit[i].GetComponent<ChestInGame>();
                    if (chest != null)
                        chest.Hit();
                }
            }
            return liste.ToArray();
        }
        public static Enemy GetInRangeTarget(Vector3 pos, float MaxRangeCheck, LayerMask layerEnemy)
        {
            //DebugCustom.Log("Find new Target");
            int rangeCast = 1;
            Enemy e = null;
            while (e == null && rangeCast <= MaxRangeCheck)
            {
                Collider2D hit = Physics2D.OverlapCircle(pos, rangeCast, layerEnemy);
                if (hit != null)
                {
                    e = hit.GetComponent<Enemy>();
                    if (e != null)
                    {
                        return e;
                    }
                }
                rangeCast++;
            }
            return e;
        }
        public static bool IsInScreen(Vector3 pos)
        {
            return pos.x > ResolutionManager.Instance.ScreenLeftEdge -1  && pos.x < ResolutionManager.Instance.ScreenRightEdge + 1
                && pos.y > ResolutionManager.Instance.ScreenBottomEdge - 1 && pos.y < ResolutionManager.Instance.ScreenTopEdge + 1;
        }
        public static bool IsLuckApply()
        {
            float luck = Player.Instance.Luck;
            return UnityEngine.Random.Range(0, 100) < luck;
        }
        public static void SpawnChestIngame()
        {
            Spawn(ObjectPooler.Instance.chestInGamePrefabs, GetRandomSpawnPos(), Quaternion.identity, null);
        }
        public static Vector3 GetRandomSpawnPos()
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.z = 0;

            Vector3 pos = Vector2.zero;
            float offset = EnemySpawner.Instance.ReSpawnOffset / 2;
            float left = ResolutionManager.Instance.ScreenLeftEdge - offset;
            float right = ResolutionManager.Instance.ScreenRightEdge + offset;
            float bottom = ResolutionManager.Instance.ScreenBottomEdge - offset;
            float top = ResolutionManager.Instance.ScreenTopEdge + offset;
            pos.x = UnityEngine.Random.Range(left, right);
            pos.y = UnityEngine.Random.Range(bottom, top);
            float xPercent = Mathf.Abs((pos.x - camPos.x) / ResolutionManager.Instance.ScreenWidth);
            float yPercent = Mathf.Abs((pos.y - camPos.y) / ResolutionManager.Instance.ScreenHigh);
            if (xPercent > yPercent)
            {
                pos.x = pos.x - Player.Instance.transform.position.x > 0 ? right : left;
            }
            else
            {
                pos.y = pos.y - Player.Instance.transform.position.y > 0 ? top : bottom;
            }
            return pos;
        }
        public static Vector3 GetRandomPosInScreen()
        {
            return new Vector3(UnityEngine.Random.Range(ResolutionManager.Instance.ScreenLeftEdge, ResolutionManager.Instance.ScreenRightEdge),
                UnityEngine.Random.Range(ResolutionManager.Instance.ScreenBottomEdge, ResolutionManager.Instance.ScreenTopEdge));
        }
        public static bool CheckSpawn(GameObject obj)
        {
            Vector3 pos = obj.transform.position;
            float offset = EnemySpawner.Instance.ReSpawnOffset;
            if (pos.x < ResolutionManager.Instance.ScreenLeftEdge - offset || pos.x > ResolutionManager.Instance.ScreenRightEdge + offset
                || pos.y < ResolutionManager.Instance.ScreenBottomEdge - offset || pos.y > ResolutionManager.Instance.ScreenTopEdge + offset)
            {
                obj.transform.position += (Player.Instance.transform.position + (Vector3)UnityEngine.Random.insideUnitCircle.normalized - obj.transform.position) * 1.6f;
                return true;
            }
            return false;
        }
        public static void ShowText(string txt)
        {

        }
        public static Sprite GetElectricShockSprite()
        {
            return ObjectPooler.Instance.ElectricShockSprite[UnityEngine.Random.Range(0, ObjectPooler.Instance.ElectricShockSprite.Length)];
        }
        #region Random
        public static bool Random_Conditional(float valueFalse, float valueTrue)
        {


            //#if UNITY_EDITOR
            //            if (Mathf.Approximately(valueFalse, 100))
            //                Debug.LogError("WTF: value false == 100%, this random is shit!!!!!!!!!");
            //#endif

            Dictionary<bool, float> dictPercent = new Dictionary<bool, float>();
            dictPercent.Add(false, valueFalse);
            dictPercent.Add(true, valueTrue);
            bool canExcute = From<bool>(dictPercent);//From(dictPercent).TakeOne();
            return canExcute;
        }
        public static List<T> GetRandomListByPercent<T>(Dictionary<T, float> dicPercent, int listCount)
        {
            Dictionary<T, float> newDic = dicPercent;
            List<T> result = new List<T>();
            if (listCount > newDic.Count)
            {
                DebugCustom.LogError("Input Wrong ");
                return null;
            }
            else if (listCount == newDic.Count)
            {
                foreach (var item in newDic)
                {
                    result.Add(item.Key);
                }
            }
            else if (listCount < newDic.Count)
            {
                for (int i = 0; i < listCount; i++)
                {
                    T element = From<T>(newDic);
                    result.Add(element);
                    newDic.Remove(element);
                }
            }
            return result;
        }
        public static T GetRandomByPercent<T>(Dictionary<T, float> dicPercent)
        {
            T result = From<T>(dicPercent);
            return result;
        }
        private static T From<T>(Dictionary<T, float> spawnRate)
        {
            WeightedRandomBag<T> bag = new WeightedRandomBag<T>();
            foreach (var item in spawnRate)
            {
                bag.AddEntry(item.Key, item.Value);
            }
            return bag.GetRandom();
            //return new WeightedRandomizer<T>(spawnRate);
        }

        public class WeightedRandomBag<T>
        {

            private struct Entry
            {
                public float accumulatedWeight;
                public T item;
            }

            private List<Entry> entries = new List<Entry>();
            private float accumulatedWeight;
            private System.Random rand = new System.Random();




            public void AddEntry(T item, float weight)
            {
                this.accumulatedWeight += weight;
                entries.Add(new Entry { item = item, accumulatedWeight = weight });
            }

            public T GetRandom()
            {
                //double r = rand.NextDouble() * accumulatedWeight;

                //foreach (Entry entry in entries)
                //{
                //    if (entry.accumulatedWeight >= r)
                //    {
                //        return entry.item;
                //    }
                //}

                var randomPoint = UnityEngine.Random.value * this.accumulatedWeight;

                for (int i = 0; i < entries.Count; i++)
                {
                    if (randomPoint < entries[i].accumulatedWeight)
                        return entries[i].item;
                    else
                        randomPoint -= entries[i].accumulatedWeight;
                }
                return default(T); //should only happen when there are no entries
            }



        }
        #endregion
    }
}
