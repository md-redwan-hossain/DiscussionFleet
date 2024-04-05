import subprocess
import os
from collections import namedtuple


DBConfig = namedtuple('DBConfig', ['db_docker_host', 'db_port', 'db_master_password', 'db_user', 'db_name', 'db_password'])

def get_db_config() -> DBConfig:
    db_docker_host = os.getenv('DB_DOCKER_HOST')
    db_port = os.getenv('DB_PORT')
    db_master_password = os.getenv('DB_MASTER_PASSWORD')
    db_user = os.getenv('DB_USER')
    db_name = os.getenv('DB_NAME')
    db_password = os.getenv('DB_PASSWORD')

    try:
        assert db_docker_host is not None, "DB_DOCKER_HOST not found"
        assert db_port is not None, "DB_PORT not found"
        assert db_master_password is not None, "DB_MASTER_PASSWORD not found"
        assert db_user is not None, "DB_USER not found"
        assert db_name is not None, "DB_NAME not found"
        assert db_password is not None, "DB_Password not found"
    except AssertionError as e:
        print(e)
        exit(1)
    else:
        return DBConfig(db_docker_host, db_port, db_master_password, db_user, db_name, db_password)


def create_login() -> None:
    env_data = get_db_config()

    sql_query = f"""
      IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE [name] = '{env_data.db_user}')
      BEGIN
          CREATE LOGIN [{env_data.db_user}] WITH PASSWORD = '{env_data.db_password}';
      END;
    """

    try:
      subprocess.check_output(["sqlcmd", "-S", f"{env_data.db_docker_host},{env_data.db_port}", "-U", "sa", "-P", env_data.db_master_password, "-Q", sql_query], stderr=subprocess.STDOUT)
    except subprocess.CalledProcessError as e:
      print(e.output.decode())
      exit(1)

def create_db() -> None:
    env_data = get_db_config()

    sql_query = f"""
      IF NOT EXISTS (SELECT *
      FROM sys.databases
      WHERE [name] = '{env_data.db_name}')
              BEGIN
          CREATE DATABASE [{env_data.db_name}];
      END;
    """

    try:
      subprocess.check_output(["sqlcmd", "-S", f"{env_data.db_docker_host},{env_data.db_port}", "-U", "sa", "-P", env_data.db_master_password, "-Q", sql_query], stderr=subprocess.STDOUT)
    except subprocess.CalledProcessError as e:
      print(e.output.decode())
      exit(1)

def create_and_assign_user() -> None:
    env_data = get_db_config()

    sql_query = f"""
      IF NOT EXISTS (SELECT *
      FROM sys.database_principals
      WHERE [name] = '{env_data.db_user}')
              BEGIN
          CREATE USER [{env_data.db_user}] FOR LOGIN [{env_data.db_user}];
          EXEC sp_addrolemember 'db_owner', '{env_data.db_user}';
      END;
    """

    try:
      subprocess.check_output(["sqlcmd", "-S", f"{env_data.db_docker_host},{env_data.db_port}", "-U", "sa", "-P", env_data.db_master_password, f"-d{env_data.db_name}", "-Q", sql_query], stderr=subprocess.STDOUT)
    except subprocess.CalledProcessError as e:
      print(e.output.decode())
      exit(1)

if __name__ == "__main__":
    create_login()
    create_db()
    create_and_assign_user()
    exit(0)