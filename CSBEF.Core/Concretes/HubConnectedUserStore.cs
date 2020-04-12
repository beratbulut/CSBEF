using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models.HubModels;

namespace CSBEF.Core.Concretes {
    public delegate Task HubConnectedUserStoreCursDelegate (IHubUserModel user);

    public static class HubConnectedUserStore {
        private static readonly List<IHubUserModel> _store = new List<IHubUserModel> ();
        private static readonly List<IHubConnectedIdModel> _store_connectedIds = new List<IHubConnectedIdModel> ();
        private static readonly object _locker = new object ();

        public static event HubConnectedUserStoreCursDelegate AddEvent;

        public static event HubConnectedUserStoreCursDelegate RemoveEvent;

        public static async Task<bool> Add (IHubUserModel user) {
            if (user == null)
                throw new ArgumentNullException (nameof (user));

            var addNewUser = false;

            try {
                lock (_locker) {
                    var connectedId = user.ConnectionId.First ();

                    var findUser = _store.FirstOrDefault (i => i.Id == user.Id);
                    if (findUser != null) {
                        findUser.ConnectionId.Add (connectedId);
                    } else {
                        addNewUser = true;
                        _store.Add (user);
                    }

                    var findId = _store_connectedIds.FirstOrDefault (i => i.ConnectionId == connectedId);
                    if (findId == null) {
                        _store_connectedIds.Add (new HubConnectedIdModel {
                            UserId = user.Id,
                                ConnectionId = connectedId
                        });
                    }

                    AddEvent?.Invoke (user);
                }
            } catch (CustomException) {
                return await Add (user).ConfigureAwait (false);
            }

            return addNewUser;
        }

        public static async Task<bool> Remove (string connectionId) {
            bool removeUser = false;

            try {
                lock (_locker) {
                    var findId = _store_connectedIds.First (i => i.ConnectionId == connectionId);
                    var findUser = _store.First (i => i.Id == findId.UserId);
                    var findId2 = findUser.ConnectionId.First (i => i == connectionId);
                    findUser.ConnectionId.Remove (findId2);
                    var userClone = (IHubUserModel) findUser.Clone ();
                    if (!findUser.ConnectionId.Any ()) {
                        removeUser = true;
                        _store.Remove (findUser);
                    }
                    RemoveEvent?.Invoke (userClone);
                }
            } catch (CustomException) {
                return await Remove (connectionId).ConfigureAwait (false);
            }

            return removeUser;
        }

        public static async Task<IList<IHubUserModel>> ConnectedUserList () {
            try {
                IList<IHubUserModel> listClone;
                lock (_locker) {
                    listClone = _store.Clone ();
                }
                return listClone;
            } catch (CustomException) {
                return await ConnectedUserList ().ConfigureAwait (false);
            }
        }

        public static async Task<IList<IHubConnectedIdModel>> ConnectedIdList () {
            try {
                IList<IHubConnectedIdModel> listClone;
                lock (_locker) {
                    listClone = _store_connectedIds.Clone ();
                }
                return listClone;
            } catch (CustomException) {
                return await ConnectedIdList ().ConfigureAwait (false);
            }
        }

        public static async Task<IHubUserModel> FindUser (string connectionId) {
            IHubUserModel user = null;

            try {
                lock (_locker) {
                    var findIdRecord = _store_connectedIds.FirstOrDefault (i => i.ConnectionId == connectionId);
                    if (findIdRecord != null) {
                        var getUser = _store.FirstOrDefault (i => i.Id == findIdRecord.UserId);
                        if (getUser != null) {
                            user = getUser;
                        }
                    }
                }
            } catch (CustomException) {
                user = await FindUser (connectionId).ConfigureAwait (false);
            }

            return user;
        }
    }
}