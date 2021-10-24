﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace WorkspaceManagerTool.Models {
    public class Script : GroupableResource {

        private string commands;

        private Options options;

        public string Commands {
            get { return commands; }
            set { SetProperty(ref commands, value); }
        }

        public Options Options {
            get { return options; }
            set { SetProperty(ref options, value); }
        }

        public Script(string name, string description, string commands, Group group, bool pinned = false) {
            Name = name;
            Description = description;
            Commands = commands;
            Group = group;
            Pinned = pinned;
            Options = new Options(ExecutionOption.KeepOpenAfterFinish);
        }

        public override bool Equals(object obj) {
            Script qa = obj as Script;
            return qa != null
                && (Name ?? "").Equals(qa.Name)
                && (Description ?? "").Equals(qa.Description)
                && (Commands ?? "").Equals(qa.Commands)
                && Group.Equals(qa.Group);
        }

        public override int GetHashCode() {
            return (Commands != null ? Commands.GetHashCode() : 0);
        }

        public void SetOptions(Options op) {
            Options = op;
        }
    }
}