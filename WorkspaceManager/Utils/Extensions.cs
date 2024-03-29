﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkspaceManagerTool.Models;

namespace WorkspaceManagerTool.Utils {
    public static class Extensions {

        public static ObservableCollection<T> ToObservableCollection<T>(this IQueryable<T> items) {
            return new ObservableCollection<T>(items);
        }

    }
}
