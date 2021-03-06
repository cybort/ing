﻿using System;
using System.Collections.Generic;

namespace Sakuno.ING.Composition
{
    public abstract class Compositor
    {
        protected Compositor()
        {
            if (Default == null)
                Default = this;
        }

        public abstract T Resolve<T>() where T : class;
        public abstract object Resolve(Type type);
        public abstract T ResolveNamed<T>(string name) where T : class;
        public abstract object ResolveNamed(Type type, string name);
        public abstract IEnumerable<WithMeta<T>> ResolveWithMetadata<T>() where T : class;

        public static Compositor Default { get; private set; }
        public static T Static<T>() where T : class => Default?.Resolve<T>();
    }
}
