# 🏢 Sistema de Gestión Inmobiliaria

Un sistema web completo desarrollado en **ASP.NET Core MVC** para la gestión integral de una inmobiliaria, con funcionalidades para administrar propiedades, contratos, pagos y usuarios.

## 🚀 Características Principales

- **Dashboard Ejecutivo**: Resumen completo con métricas clave y gráficos
- **Gestión de Propietarios**: CRUD completo con validaciones
- **Gestión de Inquilinos**: Registro y seguimiento de inquilinos
- **Gestión de Inmuebles**: Catálogo completo con imágenes y detalles
- **Gestión de Contratos**: Creación, renovación y rescisión de contratos
- **Sistema de Pagos**: Registro y seguimiento de pagos mensuales
- **Gestión de Usuarios**: Sistema de roles (Administrador/Empleado)
- **Perfil de Usuario**: Edición de datos personales y foto de perfil

## 🛠️ Tecnologías Utilizadas

- **Backend**: ASP.NET Core 8.0 MVC
- **Base de Datos**: MySQL
- **Frontend**: HTML5, CSS3, Bootstrap 5, JavaScript
- **Iconos**: Font Awesome
- **Autenticación**: Cookie Authentication

## 📋 Requisitos Previos

- Visual Studio 2022 o VS Code
- .NET 8.0 SDK
- MySQL Server 8.0+
- Git

## ⚙️ Configuración del Proyecto

### 1. Clonar el Repositorio
```bash
git clone https://github.com/lucasalonia/Inmobilaria_lab2_TPI_MGS.git
cd inmobilaria-lab2-tpi-mgs
```

### 2. Configurar la Base de Datos

El proyecto ya está conectado a una base de datos MySQL remota. **Importante**: Asegúrate de que en el `BaseRepository.cs` el parámetro `usarLocal` esté en `false` para usar la conexión a la base de datos remota.

```csharp
protected BaseRepository(bool usarLocal = false) // ← Debe estar en false
```

La configuración de conexión ya está preestablecida y lista para usar.

### 3. Restaurar Dependencias
```bash
dotnet restore
```

### 4. Ejecutar el Proyecto
```bash
dotnet run
```

La aplicación estará disponible en: `https://localhost:5001`

## 👥 Usuarios de Prueba

Para acceder al sistema, utiliza estas credenciales:

| Usuario | Contraseña | Rol | Descripción |
|---------|------------|-----|-------------|
| `empleadoGomez` | `123456` | Empleado | Usuario con permisos limitados |
| `admin` | `123456` | Administrador | Usuario con acceso completo |

### 🔐 Roles y Permisos

- **Administrador**: Acceso completo a todas las funcionalidades
- **Empleado**: Acceso limitado (sin gestión de usuarios)

## 📱 Funcionalidades por Módulo

### 🏠 Gestión de Inmuebles
- Registro de nuevas propiedades
- Carga de imágenes múltiples
- Búsqueda y filtrado
- Estados: Disponible, Alquilado, Mantenimiento

### 👥 Gestión de Personas
- **Propietarios**: Datos personales completos
- **Inquilinos**: Registro con validaciones
- Historial de contratos por persona

### 📄 Gestión de Contratos
- Creación automática de contratos
- Generación automática de pagos mensuales
- Renovación de contratos
- Rescisión con cálculo de multas
- **Auditoría completa**: Registro de quién creó/terminó cada contrato

### 💰 Sistema de Pagos
- Registro de abonos parciales
- Cálculo automático de saldos
- Estados: Pendiente, Pagado
- Historial completo de transacciones

### 👤 Gestión de Usuarios
- Sistema de autenticación seguro
- Roles y permisos
- Perfil

## 🗂️ Estructura del Proyecto

```
Inmobilaria_lab2_TPI_MGS/
├── Controllers/          # Controladores MVC
├── Models/              # Modelos de datos
│   └── ViewModels/      # Modelos para vistas
├── Views/               # Vistas
├── Repository/          # Capa de acceso a datos
├── Services/            # Lógica de negocio
├── wwwroot/            # Archivos estáticos
└── db/                 # Scripts de base de datos
```

## 🔧 Funcionalidades Técnicas

- **Arquitectura Limpia**: Separación de responsabilidades
- **Validaciones**: ModelState y Data Annotations
- **Manejo de Errores**: Try-catch con logging
- **Seguridad**: Autenticación y autorización
- **Performance**: Consultas optimizadas con paginación
- **Auditoría**: Tracking de usuarios que crean/modifican registros