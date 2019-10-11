using CSBEF.Core.Helpers;
using CSBEF.Core.Interfaces;
using CSBEF.Core.Models.HubModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CSBEF.Core.Concretes
{
    public delegate Task HubConnectedUserStoreCursDelegate(IHubUserModel user);

    public static class HubConnectedUserStore
    {
        private static List<IHubUserModel> _store = new List<IHubUserModel>();
        private static List<IHubConnectedIdModel> _store_connectedIds = new List<IHubConnectedIdModel>();
        private static bool _lock = false;

        public static event HubConnectedUserStoreCursDelegate AddEvent;

        public static event HubConnectedUserStoreCursDelegate RemoveEvent;

        public static async Task<bool> Add(IHubUserModel user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            var addNewUser = false;

            while (_lock)
            {
                Thread.Sleep(100);
            }

            _lock = true;

            try
            {
                var connectedId = user.ConnectionId.First();

                var findUser = _store.FirstOrDefault(i => i.Id == user.Id);
                if (findUser != null)
                {
                    findUser.ConnectionId.Add(connectedId);
                }
                else
                {
                    addNewUser = true;
                    _store.Add(user);
                }

                var findId = _store_connectedIds.FirstOrDefault(i => i.ConnectionId == connectedId);
                if (findId == null)
                {
                    _store_connectedIds.Add(new HubConnectedIdModel
                    {
                        UserId = user.Id,
                        ConnectionId = connectedId
                    });
                }

                AddEvent?.Invoke(user);
                _lock = false;
            }
            catch (Exception)
            {
                Thread.Sleep(100);
                return await Add(user).ConfigureAwait(false);
            }

            return addNewUser;
        }

        public static async Task<bool> Remove(string connectionId)
        {
            bool removeUser = false;

            while (_lock)
            {
                Thread.Sleep(100);
            }

            _lock = true;

            try
            {
                var findId = _store_connectedIds.First(i => i.ConnectionId == connectionId);
                var findUser = _store.First(i => i.Id == findId.UserId);
                var findId2 = findUser.ConnectionId.First(i => i == connectionId);
                findUser.ConnectionId.Remove(findId2);
                var userClone = (IHubUserModel)findUser.Clone();
                if (!findUser.ConnectionId.Any())
                {
                    removeUser = true;
                    _store.Remove(findUser);
                }
                RemoveEvent?.Invoke(userClone);
                _lock = false;
            }
            catch (Exception)
            {
                Thread.Sleep(100);
                return await Remove(connectionId).ConfigureAwait(false);
            }

            return removeUser;
        }

        public static async Task<IList<IHubUserModel>> ConnectedUserList()
        {
            while (_lock)
            {
                Thread.Sleep(100);
            }

            _lock = true;

            try
            {
                var listClone = _store.Clone();
                _lock = false;
                return listClone;
            }
            catch (Exception)
            {
                _lock = false;
                Thread.Sleep(100);
                return await ConnectedUserList().ConfigureAwait(false);
            }
        }

        public static async Task<IList<IHubConnectedIdModel>> ConnectedIdList()
        {
            while (_lock)
            {
                Thread.Sleep(100);
            }

            _lock = true;

            try
            {
                var listClone = _store_connectedIds.Clone();
                _lock = false;
                return listClone;
            }
            catch (Exception)
            {
                _lock = false;
                Thread.Sleep(100);
                return await ConnectedIdList().ConfigureAwait(false);
            }
        }

        public static async Task<IHubUserModel> FindUser(string connectionId)
        {
            while (_lock)
            {
                Thread.Sleep(100);
            }

            _lock = true;

            IHubUserModel user = null;

            try
            {
                var findIdRecord = _store_connectedIds.FirstOrDefault(i => i.ConnectionId == connectionId);
                if (findIdRecord != null)
                {
                    var getUser = _store.FirstOrDefault(i => i.Id == findIdRecord.UserId);
                    if (getUser != null)
                    {
                        user = getUser;
                    }
                }
            }
            catch (Exception)
            {
                _lock = false;
                Thread.Sleep(100);
                user = await FindUser(connectionId).ConfigureAwait(false);
            }

            _lock = false;
            return user;
        }
    }
}