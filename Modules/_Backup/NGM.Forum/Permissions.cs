﻿using System.Collections.Generic;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace NGM.Forum {
    public class Permissions : IPermissionProvider {
        public static readonly Permission ManageForums = new Permission { Description = "Manage forums for others", Name = "ManageForums" };
        public static readonly Permission ManageOwnForums = new Permission { Description = "Manage own forums", Name = "ManageOwnForums", ImpliedBy = new[] { ManageForums } };

        public static readonly Permission ViewForum = new Permission { Description = "View forum", Name = "ViewForum" };

        public static readonly Permission CreatePost = new Permission { Description = "Create a post", Name = "CreatePost" };
        public static readonly Permission ReplyPost = new Permission { Description = "Reply to a post", Name = "ReplyPost" };
        public static readonly Permission ViewPost = new Permission { Description = "View post", Name = "ViewPost" };
        public static readonly Permission ApprovingPost = new Permission { Description = "Approve or Unapprove a post", Name = "ApprovingPost" };

        public static readonly Permission DeleteOwnPost = new Permission { Description = "Delete your own post", Name = "DeleteOwnPost" };
        public static readonly Permission DeletePost = new Permission { Description = "Delete any post", Name = "DeletePost" };

        public static readonly Permission EditOwnPost = new Permission { Description = "Edit your own post", Name = "EditOwnPost" };
        public static readonly Permission EditPost = new Permission { Description = "Edit any post", Name = "EditPost" };

        public static readonly Permission MoveOwnThread = new Permission { Description = "Move your own thread to another forum", Name = "MoveOwnThread" };
        public static readonly Permission MoveThread = new Permission { Description = "Move any thread to another forum", Name = "MoveThread" };
        public static readonly Permission StickyOwnThread = new Permission { Description = "Allows you to mark your own thread as Sticky", Name = "StickyOwnThread" };
        public static readonly Permission StickyThread = new Permission { Description = "Allows you to mark any thread as Sticky", Name = "StickyThread" };

        public static readonly Permission CloseOwnThread = new Permission { Description = "Allows you to close your own thread", Name = "CloseOwnThread" };
        public static readonly Permission CloseThread = new Permission { Description = "Allows you to close any thread", Name = "CloseThread" };

        public virtual Feature Feature { get; set; }

        public static readonly Permission MetaListForums = new Permission {};// ImpliedBy = new[] { EditBlogPost, PublishBlogPost, DeleteBlogPost } };

        public static readonly Permission MetaListOwnForums = new Permission {};// ImpliedBy = new[] { EditOwnBlogPost, PublishOwnBlogPost, DeleteOwnBlogPost } };

        public IEnumerable<Permission> GetPermissions() {
            return new[] {
                ManageForums,
                ManageOwnForums,
                ViewForum,
                
                CreatePost,
                ReplyPost, 
                ViewPost,
                ApprovingPost,

                DeleteOwnPost,
                DeletePost,
                
                EditOwnPost,
                EditPost,
                
                MoveOwnThread,
                MoveThread,
                StickyOwnThread,
                StickyThread,

                CloseOwnThread,
                CloseThread,
            };
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() {

            return new[] {
                new PermissionStereotype {
                    Name = "Administrator",
                    Permissions = new[] {ManageForums}
                },
                new PermissionStereotype {
                    Name = "Editor",
                },
                new PermissionStereotype {
                    Name = "Moderator",
                },
                new PermissionStereotype {
                    Name = "Author",
                    Permissions = new[] {ManageOwnForums}
                },
                new PermissionStereotype {
                    Name = "Contributor",
                },

                /*Need to handle*/
                new PermissionStereotype {
                    Name = "Anonymous",
                    Permissions = new[] {ViewForum, ViewPost},
                },
                new PermissionStereotype {
                    Name = "Authenticated",
                    Permissions = new[] {ViewForum, ViewPost, CreatePost, ReplyPost},
                },
            };
        }
    }
}