using Demo.Com;
using Demo.Config;
using Demo.Help;
using DG.Tweening;
using LCECS.Core.ECS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Demo.System
{
    //特效系统
    public class EffectSystem : BaseSystem
    {
        private EffectConfig effectConfig = null;
        private EffectCom effectCom;
        private GameObjectCom goCom;
        protected override List<Type> RegListenComs()
        {
            effectConfig = GameConfigHelp.GetConfig<EffectConfig>(ConfigType.Effect);
            return new List<Type>(){typeof(EffectCom),typeof(GameObjectCom)};
        }

        protected override void HandleComs(List<BaseCom> comList)
        {
            if (effectCom==null)
                effectCom= GetCom<EffectCom>(comList[0]);
            if (goCom == null)
                goCom = GetCom<GameObjectCom>(comList[1]);
            HandleEffect(effectCom);
            RecycleEffect(effectCom);
        }

        private void HandleEffect(EffectCom effectCom)
        {
            if (effectCom.CurrShowEffectId<=0)
            {
                return;
            }
            //冲刺拖尾
            if (effectCom.CurrShowEffectId==1001)
            {
                HandleDashEffect(effectCom);
            }
            else
            {
                HandleNormalEffect(effectCom);
            }

            effectCom.CurrShowEffectId = 0;
        }

        //回收特效
        private void RecycleEffect(EffectCom effectCom)
        {
            foreach (var item in effectCom.CurrShowEffects.Keys)
            {
                EffectData effectData = effectCom.CurrShowEffects[item];
                for (int i = 0; i < effectData.EffectGos.Count; i++)
                {
                    //超时
                    if (Time.time > effectData.EffectGos[i].Time+ effectData.Info.ContinueTime)
                    {
                        PushEffectGoInCache(effectCom, effectData.Info.Id, effectData.EffectGos[i]);
                        effectData.EffectGos.RemoveAt(i);
                    }
                }
            }
        }

        #region 公共方法
        //将特效加入缓存
        public void PushEffectGoInCache(EffectCom effectCom, int effectId, EffectGo effectGo)
        {
            effectGo.Go.SetActive(false);
            if (effectCom.CacheEffects.ContainsKey(effectId))
            {
                effectCom.CacheEffects[effectId].EffectGos.Add(effectGo);
            }
            else
            {
                EffectData effectData = new EffectData();
                effectData.Info = GetEffectInfo(effectId);
                effectData.EffectGos.Add(effectGo);
                effectCom.CacheEffects.Add(effectId, effectData);
            }
        }

        //获得创建的特效节点
        private EffectGo GetEffectGo(EffectCom effectCom, int effectId,ref EffectInfo info)
        {
            //创建或者从缓存中拿到预制体
            EffectGo effectGo;
            if (effectCom.CacheEffects.ContainsKey(effectId))
            {
                EffectData effect = effectCom.CacheEffects[effectId];
                effectGo = effect.EffectGos[0];
                effect.EffectGos.RemoveAt(0);
                if (effect.EffectGos.Count <= 0)
                {
                    effectCom.CacheEffects.Remove(effectId);
                }
            }
            else
            {
                effectGo = CreateEffectGo(effectId);
            }
            effectGo.Time = Time.time;

            //保存在当前显示的字典中
            if (effectCom.CurrShowEffects.ContainsKey(effectCom.CurrShowEffectId))
            {
                info = effectCom.CurrShowEffects[effectCom.CurrShowEffectId].Info;
                effectCom.CurrShowEffects[effectCom.CurrShowEffectId].EffectGos.Add(effectGo);
            }
            else
            {
                EffectData effectData = new EffectData();
                effectData.Info = GetEffectInfo(effectCom.CurrShowEffectId);
                effectData.EffectGos.Add(effectGo);
                effectCom.CurrShowEffects.Add(effectCom.CurrShowEffectId, effectData);
                info = effectData.Info;
            }

            effectGo.Go.SetActive(true);
            return effectGo;
        }

        private EffectGo CreateEffectGo(int effectId)
        {
            EffectInfo effectInfo = GetEffectInfo(effectId);
            if (effectInfo == null)
            {
                return null;
            }
            EffectGo effectGo = new EffectGo
            {
                Time = Time.time,
                Go = UnityEngine.Object.Instantiate(effectInfo.Prefab)
            };
            effectGo.Go.SetActive(false);
            effectGo.Go.transform.SetParent(goCom.Tran);
            effectGo.Go.transform.position = Vector3.zero;
            return effectGo;
        }

        private EffectInfo GetEffectInfo(int effectId)
        {
            EffectInfo effectInfo = null;
            for (int i = 0; i < effectConfig.EffectList.Count; i++)
            {
                if (effectConfig.EffectList[i].Id == effectId)
                {
                    effectInfo = effectConfig.EffectList[i];
                    break;
                }
            }
            return effectInfo;
        }
        #endregion

        #region 处理冲刺拖尾特效
        private void HandleDashEffect(EffectCom effectCom)
        {
            Entity entity = LCECS.ECSLocate.ECS.GetEntity(effectCom.EntityId);
            GameObjectCom goCom = entity.GetCom<GameObjectCom>();
            Transform entityTran = goCom.Tran;

            //获得特效节点
            EffectInfo info   = null;
            EffectGo effectGo = GetEffectGo(effectCom, effectCom.CurrShowEffectId,ref info);

            //动画队列
            Sequence s = DOTween.Sequence();
            for (int i = 0; i < effectGo.Go.transform.childCount; i++)
            {
                Transform dashGo = effectGo.Go.transform.GetChild(i);
                dashGo.gameObject.SetActive(false);
                s.AppendCallback(() => InitDashGo(entityTran,dashGo,i));
                //间隔后，才赋值位置
                s.AppendInterval(info.IntervalTime);
            }
        }

        //初始化冲刺的节点
        private void InitDashGo(Transform tran,Transform dashGo, int index)
        {
            SpriteRenderer sp           = tran.GetComponent<SpriteRenderer>();
            SpriteRenderer dashSp       = dashGo.GetComponent<SpriteRenderer>();
            //位置
            dashGo.transform.position   = tran.position;
            //方向
            dashSp.flipX                = sp.flipX;
            //图片
            dashSp.sprite               = sp.sprite;
            dashGo.gameObject.SetActive(true);
        }

        //渐隐冲刺节点
        private void DashFadeSprite(Transform current,float hideTime)
        {
            current.GetComponent<SpriteRenderer>().DOColor(new Color(255, 255, 255, 0), hideTime);
        }
        #endregion

        #region 处理一般的特效

        public void HandleNormalEffect(EffectCom effectCom)
        {
            Entity entity = LCECS.ECSLocate.ECS.GetEntity(effectCom.EntityId);
            GameObjectCom goCom = entity.GetCom<GameObjectCom>();
            Transform entityTran = goCom.Tran;

            //获得特效节点
            EffectInfo info = null;
            EffectGo effectGo = GetEffectGo(effectCom, effectCom.CurrShowEffectId, ref info);

            //设置位置
            effectGo.Go.transform.position = entityTran.position + effectCom.ShowPos;
            effectGo.Go.SetActive(true);
        }

        #endregion
    }
}
