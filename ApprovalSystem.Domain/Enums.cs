namespace ApprovalSystem.Domain;

public enum AccessType { Read = 1, Edit = 2 }
public enum RequestStatus { Pending = 1, Approved = 2, Rejected = 3 }
public enum DecisionType { Approve = 1, Reject = 2 }
public enum UserRole { User = 1, Approver = 2, Admin = 3 }
