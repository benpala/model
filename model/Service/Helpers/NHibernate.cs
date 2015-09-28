﻿using NHibernate;
using NHibernate.Cfg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Immobilus.Logic.Services.Helpers
{
    public class NHibernateConnexion
    {
        private static ISessionFactory _sessionFactory;
        private static readonly bool IS_FLUENT = false;

        static NHibernateConnexion()
        {
            _sessionFactory = new Configuration().AddAssembly(typeof(NHibernateConnexion).Assembly).BuildSessionFactory(); 
        }
        private static ISessionFactory SessionFactory
        {
            get
            {
                return _sessionFactory;
            }
        }
        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}