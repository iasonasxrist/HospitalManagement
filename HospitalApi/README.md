# Hospital Management API - Modular Monolith

A comprehensive .NET 9 Web API for hospital management with modular monolith architecture, role-based access control, patient management, medical records, vital signs monitoring, and real-time notifications for critical patient issues.

## 🏥 **Enhanced Features**

### 🔐 **Role-Based Access Control (RBAC)**
- **Admin Role**: Full system access, user management, system administration
- **Doctor Role**: Patient care, medical records, vital signs, prescriptions
- **Nurse Role**: Patient monitoring, vital signs recording, basic patient care
- **Custom Authorization Attributes**: `[AuthorizeRoles(UserRole.Admin, UserRole.Doctor)]`
- **API Protection**: All endpoints protected with appropriate role requirements

### 👥 **Enhanced Patient Management**
- **Complete Patient Profiles**: Room, department, blood type, condition tracking
- **Critical Patient Tracking**: Real-time alerts with severity levels
- **Patient Search & Filtering**: By name, room, department, critical status
- **Emergency Contact Information**: Comprehensive contact details
- **Medical History & Allergies**: Detailed patient background

### 📊 **Vital Signs Monitoring System**
- **Comprehensive Vital Signs**: Temperature, blood pressure, heart rate, oxygen saturation, respiratory rate, weight, height
- **Automatic Severity Detection**: Normal, Elevated, High, Critical levels
- **Real-time Critical Alerts**: Automatic notification system for dangerous values
- **Blood Pressure Monitoring**: Critical alerts for values ≥180/110 mmHg
- **Oxygen Saturation Tracking**: Critical alerts for values <90%
- **Temperature Monitoring**: Alerts for abnormal temperature ranges

### 📋 **Advanced Medical Records**
- **Comprehensive Documentation**: Diagnosis, symptoms, treatment, prescriptions
- **Vital Signs Integration**: Automatic linking with patient vital signs
- **Critical Condition Flagging**: Real-time status updates
- **Medical History Tracking**: Complete patient care timeline

### 🔔 **Intelligent Notification System**
- **Real-time Critical Alerts**: Automatic detection and notification
- **Priority-based System**: Low, Normal, High, Critical priorities
- **Severity Levels**: Normal, Elevated, High, Critical severity
- **Alert Types**: 
  - Critical Patient Alerts
  - Vital Sign Alerts
  - Medical Record Updates
  - Blood Pressure Alerts
  - Oxygen Saturation Alerts
  - System Alerts
- **Room & Department Tracking**: Location-based notifications

### 🧪 **Additional Medical Modules**
- **Prescription Management**: Medication tracking and management
- **Lab Results**: Laboratory test results with severity tracking
- **Progress Notes**: Clinical progress documentation
- **Appointment Scheduling**: Patient appointment management

## 🏗️ **Modular Monolith Architecture**

### **Core Modules**
```
HospitalApi/
├── Models/           # Domain entities
├── DTOs/            # Data transfer objects
├── Services/        # Business logic services
├── Controllers/     # API endpoints
├── Data/           # Data access layer
├── Authorization/  # Role-based access control
└── Tests/          # Comprehensive test suite
```

### **Service Layer**
- **NotificationService**: Critical alert management
- **VitalSignService**: Vital signs monitoring with automatic severity detection
- **UserService**: User management and authentication
- **PatientService**: Patient care management
- **MedicalRecordService**: Medical documentation

## 🧪 **Comprehensive Testing Suite**

### **xUnit Test Coverage**
- **Controller Tests**: All API endpoints tested
- **Service Tests**: Business logic validation
- **Authorization Tests**: Role-based access verification
- **Integration Tests**: End-to-end functionality
- **Critical Alert Tests**: Notification system validation

### **Test Categories**
- **User Management Tests**: Authentication, authorization, CRUD operations
- **Vital Signs Tests**: Normal values, critical alerts, severity detection
- **Patient Management Tests**: CRUD operations, critical status tracking
- **Notification Tests**: Alert creation, priority handling, role-based access

## 🚨 **Critical Alert System**

### **Automatic Detection**
The system automatically detects and alerts on:

1. **Blood Pressure Critical Values**:
   - Systolic ≥ 180 mmHg or Diastolic ≥ 110 mmHg
   - Triggers immediate critical alert to all medical staff

2. **Oxygen Saturation Critical Values**:
   - SpO2 < 90% triggers critical alert
   - SpO2 < 95% triggers high priority alert

3. **Heart Rate Abnormalities**:
   - < 50 bpm or > 120 bpm triggers high priority alert
   - < 60 bpm or > 100 bpm triggers elevated alert

4. **Temperature Abnormalities**:
   - < 95°F or > 103°F triggers high priority alert
   - < 97°F or > 100.4°F triggers elevated alert

### **Alert Examples**
```
CRITICAL BLOOD PRESSURE ALERT
Patient: Michael Chen
Room: B-205 - Cardiology
Blood pressure reading of 180/110 mmHg exceeds critical threshold

Current Value: 180/110 mmHg
Normal Range: 120/80 mmHg
Severity: Critical
```

## 🔧 **Technology Stack**

- **.NET 9** - Latest .NET framework
- **Entity Framework Core** - ORM for database operations
- **PostgreSQL** - Primary database
- **BCrypt.Net-Next** - Secure password hashing
- **xUnit** - Comprehensive testing framework
- **FluentAssertions** - Readable test assertions
- **Moq** - Mocking framework for tests
- **Swagger/OpenAPI** - API documentation

## 📊 **Database Schema**

### **Enhanced Entities**
- **Users**: Role-based access with Admin/Doctor/Nurse roles
- **Patients**: Complete profiles with room, department, blood type
- **VitalSigns**: Comprehensive monitoring with automatic severity detection
- **MedicalRecords**: Detailed medical documentation
- **Notifications**: Priority and severity-based alert system
- **Prescriptions**: Medication management
- **LabResults**: Laboratory test tracking
- **ProgressNotes**: Clinical progress documentation

## 🚀 **API Endpoints**

### **Authentication & Authorization**
- `POST /api/users/login` - User authentication
- `GET /api/users` - Get all users (Admin only)
- `POST /api/users` - Create user (Admin only)
- `GET /api/users/doctors` - Get doctors (All roles)
- `GET /api/users/nurses` - Get nurses (All roles)

### **Patient Management**
- `GET /api/patients` - Get all patients with filtering
- `POST /api/patients` - Create patient
- `GET /api/patients/critical` - Get critical patients
- `POST /api/patients/{id}/mark-critical` - Mark patient as critical
- `POST /api/patients/{id}/mark-stable` - Mark patient as stable

### **Vital Signs Management**
- `POST /api/vitalsigns` - Record vital signs (Doctor/Nurse)
- `GET /api/vitalsigns/patient/{id}` - Get patient vital signs
- `GET /api/vitalsigns/critical` - Get critical vital signs
- `GET /api/vitalsigns/latest/{id}` - Get latest vital signs

### **Medical Records**
- `POST /api/medicalrecords` - Create medical record
- `GET /api/medicalrecords/critical` - Get critical records
- `GET /api/medicalrecords/patient/{id}` - Get patient records

### **Notifications**
- `GET /api/notifications` - Get all notifications
- `GET /api/notifications/critical` - Get critical alerts
- `POST /api/notifications/{id}/mark-read` - Mark as read

## 🛠️ **Setup Instructions**

### **Prerequisites**
- .NET 9 SDK
- PostgreSQL database
- Visual Studio 2022 or VS Code

### **Installation**

1. **Clone and Setup**
   ```bash
   git clone <repository-url>
   cd HospitalApi
   dotnet restore
   ```

2. **Database Configuration**
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=HospitalDb;Username=your_username;Password=your_password"
     }
   }
   ```

3. **Run the Application**
   ```bash
   dotnet run
   ```

4. **Run Tests**
   ```bash
   cd HospitalApi.Tests
   dotnet test
   ```

5. **Access Points**
   - API: `https://localhost:7212` or `http://localhost:5212`
   - Swagger UI: `https://localhost:7212/swagger`
   - Test Coverage: `dotnet test --collect:"XPlat Code Coverage"`

### **Default Users**

| Username    | Password  | Role   | Access Level                     |
| ----------- | --------- | ------ | -------------------------------- |
| admin       | admin123  | Admin  | Full system access               |
| dr.smith    | doctor123 | Doctor | Patient care & medical records   |
| nurse.jones | nurse123  | Nurse  | Patient monitoring & vital signs |

## 🧪 **Testing the API**

### **Role-Based Testing**
Use the provided `HospitalApi.http` file with role headers:

```http
# Admin access
X-User-Role: Admin

# Doctor access  
X-User-Role: Doctor

# Nurse access
X-User-Role: Nurse
```

### **Critical Alert Testing**
1. **Create critical vital signs** to trigger automatic alerts
2. **Mark patients as critical** to test notification system
3. **Test role-based access** to verify authorization

### **Example Critical Alert Test**
```http
POST /api/vitalsigns
X-User-Role: Nurse
Content-Type: application/json

{
  "patientId": 2,
  "recordedByUserId": 3,
  "bloodPressureSystolic": 180,
  "bloodPressureDiastolic": 110,
  "notes": "Critical blood pressure alert"
}
```

## 🔒 **Security Features**

- **BCrypt Password Hashing** for secure password storage
- **Role-Based Access Control** for all API endpoints
- **Input Validation** and data sanitization
- **Soft Delete** for data integrity
- **CORS Configuration** for cross-origin requests
- **Authorization Headers** for role-based access

## 📈 **Future Enhancements**

- **JWT Token Authentication** with refresh tokens
- **Real-time WebSocket Notifications** for instant alerts
- **Advanced Appointment Scheduling** with conflict detection
- **Prescription Management** with drug interaction checking
- **Lab Results Integration** with automated result processing
- **Audit Logging** for compliance and security
- **Advanced Reporting** and analytics dashboard
- **Mobile App Support** with push notifications
- **Integration with Medical Devices** for automatic vital signs
- **AI-Powered Risk Assessment** for patient monitoring

## 🤝 **Contributing**

1. Fork the repository
2. Create a feature branch
3. Add comprehensive tests for new features
4. Ensure all tests pass
5. Submit a pull request with detailed description

## 📄 **License**

This project is licensed under the MIT License.

---

**🏥 Built for Healthcare Excellence - Comprehensive, Secure, and Reliable Hospital Management System** 