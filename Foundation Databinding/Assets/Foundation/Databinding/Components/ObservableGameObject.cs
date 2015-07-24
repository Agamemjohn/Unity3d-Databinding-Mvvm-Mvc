// -------------------------------------
//  Domain		: Avariceonline.com
//  Author		: Nicholas Ventimiglia & John Emerson
//  Product		: Unity3d Foundation
//  Published		: 2015
//  -------------------------------------
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Foundation.Databinding.Model;
using Foundation.Databinding.View;
using UnityEngine;
#if UNITY_WSA
using System.Runtime.CompilerServices;
#endif

namespace Foundation.Databinding.Components
{
    /// <summary>
    /// Implements the IObservableModel for all components on this GameObject
    /// </summary>
    [AddComponentMenu("Foundation/Databinding/ObservableGameObject")]
    public class ObservableGameObject : MonoBehaviour, IObservableModel
    {
        private Action<ObservableMessage> _onBindingEvent = delegate { };
        public event Action<ObservableMessage> OnBindingUpdate
        {
            add
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Combine(_onBindingEvent, value);
            }
            remove
            {
                _onBindingEvent = (Action<ObservableMessage>)Delegate.Remove(_onBindingEvent, value);
            }
        }

        private Dictionary<string, IObservableModel> _binderMap = new Dictionary<string, IObservableModel>();

        private ObservableMessage _bindingMessage;

        private IObservableModel FindModel(string memberName)
        {
            IObservableModel outModel = null;
            if (_binderMap.TryGetValue(memberName, out outModel) == false)
            {
                foreach (IObservableModel model in gameObject.GetInterfaces<IObservableModel>())
                {
                    if (model != (IObservableModel)this)
                    {
                        if(model.GetType().GetRuntimeMember(memberName) != null)
                        {
                            outModel = model;
                            outModel.OnBindingUpdate += RelayBindingUpdate;
                            break;
                        }
                    }
                }

                _binderMap.Add(memberName, outModel);

                if(outModel == null)
                    UnityEngine.Debug.LogError("Member not found ! " + memberName + " " + GetType());
            }
            return outModel;
        }

        /// <summary>
        /// 
        /// </summary>
        protected bool IsApplicationQuit { get; private set; }

        /// <summary>
        /// Virtual, Initializes Binder
        /// </summary>
        protected virtual void Awake()
        {
            if (_bindingMessage == null)
                _bindingMessage = new ObservableMessage { Sender = this };
        }

        /// <summary>
        /// Virtual, Disposes
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (IsApplicationQuit)
                return;

            Dispose();
        }

        [HideInInspector]
        public virtual void Dispose()
        {
            if (_bindingMessage != null)
            {
                _bindingMessage.Dispose();
            }

            foreach(var model in _binderMap.Values)
            {
                if(model != null)
                    model.OnBindingUpdate -= RelayBindingUpdate;
            }

            _bindingMessage = null;
        }

        public void RaiseBindingUpdate(string memberName, object parameter)
        {
            if (_bindingMessage == null)
                _bindingMessage = new ObservableMessage { Sender = this };

            RelayBindingUpdate(_bindingMessage);
        }

        void RelayBindingUpdate(ObservableMessage message)
        {
            if (_onBindingEvent != null)
            {
                _onBindingEvent(message);
            }
        }

        [HideInInspector]
        public void Command(string memberName)
        {
            IObservableModel model = FindModel(memberName);

            if (model != null)
            {
                model.Command(memberName);
            }
            else
            {
                UnityEngine.Debug.LogError("Member not found ! " + memberName + " " + GetType());
            }
        }

        public void Command(string memberName, object parameter)
        {
            IObservableModel model = FindModel(memberName);

            if (model != null)
            {
                model.Command(memberName, parameter);
            }
            else
            {
                UnityEngine.Debug.LogError("Member not found ! " + memberName + " " + GetType());
            }
        }

        [HideInInspector]
        public object GetValue(string memberName)
        {
            IObservableModel model = FindModel(memberName);

            if (model != null)
            {
                return model.GetValue(memberName);
            }
            else
            {
                UnityEngine.Debug.LogError("Member not found ! " + memberName + " " + GetType());
                return null;
            }
        }

        public object GetValue(string memberName, object parameter)
        {
            IObservableModel model = FindModel(memberName);

            if (model != null)
            {
                return model.GetValue(memberName, parameter);
            }
            else
            {
                UnityEngine.Debug.LogError("Member not found ! " + memberName + " " + GetType());
                return null;
            }
        }

        public virtual void NotifyProperty(string memberName, object parameter)
        {
            RaiseBindingUpdate(memberName, parameter);
        }

        protected virtual void OnApplicationQuit()
        {
            IsApplicationQuit = true;
        }

#if !UNITY_WSA
        /// <summary>
        /// Mvvm light set method
        /// </summary>
        /// <remarks>
        /// https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/issues/3
        /// https://github.com/negue
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueHolder"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T valueHolder, T value, string propName = null)
        {
            var same =  EqualityComparer<T>.Default.Equals(valueHolder, value);

            if (!same)
            {
                if (string.IsNullOrEmpty(propName))
                {
                    // get call stack
                    var stackTrace = new StackTrace();
                    // get method calls (frames)
                    var stackFrames = stackTrace.GetFrames().ToList();

                    if (propName == null && stackFrames.Count > 1)
                    {
                        propName = stackFrames[1].GetMethod().Name.Replace("set_", "");
                    }
                }

                valueHolder = value;

                NotifyProperty(propName, value);

                return true;
            }

            return false;
        }
#else
        /// <summary>
        /// Mvvm light set method
        /// </summary>
        /// <remarks>
        /// https://github.com/NVentimiglia/Unity3d-Databinding-Mvvm-Mvc/issues/3
        /// https://github.com/negue
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="valueHolder"></param>
        /// <param name="value"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        protected bool Set<T>(ref T valueHolder, T value, [CallerMemberName] string propName = null)
        {
            var same = EqualityComparer<T>.Default.Equals(valueHolder, value);

            if (!same)
            {
                NotifyProperty(propName, value);
                valueHolder = value;
                return true;
            }

            return false;
        }
#endif
    }
}
