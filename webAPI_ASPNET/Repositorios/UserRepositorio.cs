﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using webAPI_ASPNET.Data;
using webAPI_ASPNET.Models;
using webAPI_ASPNET.Repositorios.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace webAPI_ASPNET.Repositorios
{
    public class UserRepositorio : IUser
    {
        private readonly AppDbContext _dbContext;
        public UserRepositorio(AppDbContext AppDbContext) 
        {
            _dbContext = AppDbContext;
        }
        public async Task<List<UserWithDepartment>> getAll()
        {
            return await (from u in _dbContext.User
                          join ud in _dbContext.DepartmentRelation on u.ID equals ud.IDUSER into userDepartments
                          from ud in userDepartments.DefaultIfEmpty()
                          join d in _dbContext.Department on ud.IDDEPARTMENT equals d.ID into departments
                          from d in departments.DefaultIfEmpty() 
                          select new UserWithDepartment
                          {
                              User = u,
                              DepartmentRelation = ud, 
                              Department = d 
                          }).ToListAsync();

        }

        public async Task<User> getById(int id)
        {
            return await _dbContext.User.FirstOrDefaultAsync(x => x.ID == id);
        }

        public async Task<UserWithDepartment> getUserIDepartmentById(int id)
        {
            return await (from u in _dbContext.User
                                            join ud in _dbContext.DepartmentRelation on u.ID equals ud.IDUSER into userDepartments
                                            from ud in userDepartments.DefaultIfEmpty() 
                                            join d in _dbContext.Department on ud.IDDEPARTMENT equals d.ID into departments
                                            from d in departments.DefaultIfEmpty() 
                                            where u.ID == id
                                            select new UserWithDepartment
                                            {
                                                User = u,
                                                DepartmentRelation = ud, 
                                                Department = d 
                                            }).FirstOrDefaultAsync();

        }

        public async Task<UserWithDepartment> getUserIDepartmentByUsernameAndPassword(string username, string password)
        {
            return await (from u in _dbContext.User
                          join ud in _dbContext.DepartmentRelation on u.ID equals ud.IDUSER
                          join d in _dbContext.Department on ud.IDDEPARTMENT equals d.ID
                          where u.USERNAME == username && u.PASSWORD == password
                          select new UserWithDepartment
                          {
                              User = u,
                              DepartmentRelation = ud,
                              Department = d
                          }).FirstOrDefaultAsync();
        }

        public async Task<UserWithButton> getUserIButtonById(int id)
        {
            return await (from u in _dbContext.User
                          join ud in _dbContext.ButtonRelation on u.ID equals ud.IDUSER into userButtons
                          from ud in userButtons.DefaultIfEmpty()
                          join d in _dbContext.Buttons on ud.IDBUTTON equals d.ID into buttons
                          from d in buttons.DefaultIfEmpty() 
                          where ud.ID == id
                          select new UserWithButton 
                          {
                              User = u,
                              ButtonRelation = ud,
                              Button = d 
                          }).FirstOrDefaultAsync();
        }

        public async Task<User> post(User user)
        {
            await _dbContext.User.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<User> put(User user, int id)
        {
            User userById = await getById(id);
            if (userById == null)
            {
                throw new Exception($"Usuario para o ID: {id} não foi encontrado");
            }

            userById.USERNAME = user.USERNAME;
            userById.PASSWORD = user.PASSWORD;  
            userById.EMAIL = user.EMAIL;
            userById.FULLNAME = user.FULLNAME;

            _dbContext.User.Update(userById);
            await _dbContext.SaveChangesAsync();

            return userById;
        }

        public async Task<bool> delete(int id)
        {
            User userById = await getById(id);
            if (userById == null)
            {
                throw new Exception($"Usuario para o ID: {id} não foi encontrado");
            }

            _dbContext.User.Remove(userById);
            await _dbContext.SaveChangesAsync();

            return true;

        }

        public UserLogin Authenticate(string username, string password)
        {
            string query = $@"
                            SELECT U.ID, U.USERNAME, U.PASSWORD, U.FULLNAME, U.EMAIL, D.ID AS IDDEPARTMENT, D.DEPARTMENTNAME 
                            FROM USERS U 
                            INNER JOIN DEPARTMENTRELATION DR ON DR.IDUSER = U.ID
                            INNER JOIN DEPARTMENT D ON D.ID = DR.IDDEPARTMENT
                            WHERE U.USERNAME = '{username}' AND U.PASSWORD = '{password}'";

            var user = _dbContext.UserLogin.FromSqlRaw(query).SingleOrDefault();

            return user;
        }
    }
}
