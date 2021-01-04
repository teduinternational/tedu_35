import { Component, OnInit, OnDestroy } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { NotificationService, UtilitiesService, CommandsService, RolesService } from '@app/shared/services';
import { SystemConstants, MessageConstants } from '@app/shared/constants';
import { TreeNode } from 'primeng/api/treenode';
import { PermissionsService } from '@app/shared/services/permissions.service';
import { PermissionUpdateRequest } from '@app/shared/models';
import { Permission } from '@app/shared/models/permission.model';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-permissions',
  templateUrl: './permissions.component.html',
  styleUrls: ['./permissions.component.css']
})
export class PermissionsComponent implements OnInit, OnDestroy {

  private subscription = new Subscription();

  public bsModalRef: BsModalRef;
  public blockedPanel = false;

  public functions: any[];
  public flattenFunctions: any[] = [];
  public selectedRole: any = {
    id: null
  };
  public roles: any[] = [];
  public commands: any[] = [];

  public selectedViews: string[] = [];
  public selectedCreates: string[] = [];
  public selectedUpdates: string[] = [];
  public selectedDeletes: string[] = [];
  public selectedApproves: string[] = [];

  public isSelectedAllViews = false;
  public isSelectedAllCreates = false;
  public isSelectedAllUpdates = false;
  public isSelectedAllDeletes = false;
  public isSelectedAllApproves = false;

  constructor(

    private permissionsService: PermissionsService,
    private rolesService: RolesService,
    private commandsService: CommandsService,
    private _notificationService: NotificationService,
    private _utilityService: UtilitiesService) {
  }


  ngOnInit() {
    this.loadAllRoles();
    this.loadData(this.selectedRole.id);
  }

  changeRole($event: any) {
    if ($event.value != null) {
      this.loadData($event.value.id);
    } else {
      this.functions = [];
    }
  }
  public reloadData() {
    this.loadData(this.selectedRole.id);
  }
  public savePermission() {
    if (this.selectedRole.id == null) {
      this._notificationService.showError('Bạn chưa chọn nhóm quyền.');
      return;
    }
    const listPermissions: Permission[] = [];
    this.selectedCreates.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.CREATE_ACTION
      });
    });
    this.selectedUpdates.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.UPDATE_ACTION
      });
    });
    this.selectedDeletes.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.DELETE_ACTION
      });
    });
    this.selectedViews.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.VIEW_ACTION
      });
    });

    this.selectedApproves.forEach(element => {
      listPermissions.push({
        functionId: element,
        roleId: this.selectedRole.id,
        commandId: SystemConstants.APPROVE_ACTION
      });
    });
    const permissionsUpdateRequest = new PermissionUpdateRequest();
    permissionsUpdateRequest.permissions = listPermissions;
    this.subscription.add(this.permissionsService.save(this.selectedRole.id, permissionsUpdateRequest)
      .subscribe(() => {
        this._notificationService.showSuccess(MessageConstants.UPDATED_OK_MSG);

        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
  loadData(roleId) {
    if (roleId != null) {
      this.blockedPanel = true;
      this.subscription.add(this.permissionsService.getFunctionWithCommands()
        .subscribe((response: any) => {
          const unflattering = this._utilityService.UnflatteringForTree(response);
          this.functions = <TreeNode[]>unflattering;
          this.flattenFunctions = response;
          this.fillPermissions(roleId);
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }, error => {
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        }));
    }

  }
  checkChanged(checked: boolean, commandId: string, functionId: string, parentId: string) {
    if (commandId === SystemConstants.VIEW_ACTION) {
      if (checked) {
        this.selectedViews.push(functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedViews.push(...childFunctions);
        } else {
          if (this.selectedViews.filter(x => x === parentId).length === 0) {
            this.selectedViews.push(parentId);
          }
        }
      } else {
        this.selectedViews = this.selectedViews.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedViews = this.selectedViews.filter(function (el) {
            return !childFunctions.includes(el);
          });
        }
      }
    } else if (commandId === SystemConstants.CREATE_ACTION) {
      if (checked) {
        this.selectedCreates.push(functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedCreates.push(...childFunctions);
        } else {
          if (this.selectedCreates.filter(x => x === parentId).length === 0) {
            this.selectedCreates.push(parentId);
          }
        }
      } else {
        this.selectedCreates = this.selectedCreates.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedCreates = this.selectedCreates.filter(function (el) {
            return !childFunctions.includes(el);
          });
        }
      }
    } else if (commandId === SystemConstants.UPDATE_ACTION) {
      if (checked) {
        this.selectedUpdates.push(functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedUpdates.push(...childFunctions);
        } else {
          if (this.selectedUpdates.filter(x => x === parentId).length === 0) {
            this.selectedUpdates.push(parentId);
          }
        }
      } else {
        this.selectedUpdates = this.selectedUpdates.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedUpdates = this.selectedUpdates.filter(function (el) {
            return !childFunctions.includes(el);
          });
        }
      }
    } else if (commandId === SystemConstants.DELETE_ACTION) {
      if (checked) {
        this.selectedDeletes.push(functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedDeletes.push(...childFunctions);
        } else {
          if (this.selectedDeletes.filter(x => x === parentId).length === 0) {
            this.selectedDeletes.push(parentId);
          }
        }
      } else {
        this.selectedDeletes = this.selectedDeletes.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedDeletes = this.selectedDeletes.filter(function (el) {
            return !childFunctions.includes(el);
          });
        }
      }
    } else if (commandId === SystemConstants.APPROVE_ACTION) {
      if (checked) {
        this.selectedApproves.push(functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedApproves.push(...childFunctions);
        } else {
          if (this.selectedApproves.filter(x => x === parentId).length === 0) {
            this.selectedApproves.push(parentId);
          }
        }
      } else {
        this.selectedApproves = this.selectedApproves.filter(x => x !== functionId);
        if (parentId === null) {
          const childFunctions = this.flattenFunctions.filter(x => x.parentId === functionId).map(x => x.id);
          this.selectedApproves = this.selectedApproves.filter(function (el) {
            return !childFunctions.includes(el);
          });
        }
      }
    }

  }
  selectAll(checked: boolean, uniqueCode: string) {
    if (uniqueCode === SystemConstants.VIEW_ACTION) {
      this.selectedViews = [];
      if (checked) {
        this.selectedViews.push(...this.flattenFunctions.map(x => x.id));
      }
    } else if (uniqueCode === SystemConstants.CREATE_ACTION) {
      this.selectedCreates = [];
      if (checked) {
        this.selectedCreates.push(...this.flattenFunctions.map(x => x.id));
      }
    } else if (uniqueCode === SystemConstants.UPDATE_ACTION) {
      this.selectedUpdates = [];
      if (checked) {
        this.selectedUpdates.push(...this.flattenFunctions.map(x => x.id));
      }
    } else if (uniqueCode === SystemConstants.DELETE_ACTION) {
      this.selectedDeletes = [];
      if (checked) {
        this.selectedDeletes.push(...this.flattenFunctions.map(x => x.id));
      }
    } else if (uniqueCode === SystemConstants.APPROVE_ACTION) {
      this.selectedApproves = [];
      if (checked) {
        this.selectedApproves.push(...this.flattenFunctions.map(x => x.id));
      }
    }
  }
  fillPermissions(roleId: any) {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getRolePermissions(roleId)
      .subscribe((response: Permission[]) => {
        this.selectedCreates = [];
        this.selectedUpdates = [];
        this.selectedDeletes = [];
        this.selectedViews = [];
        this.selectedApproves = [];
        response.forEach(element => {
          if (element.commandId === SystemConstants.CREATE_ACTION) {
            this.selectedCreates.push(element.functionId);
          }
          if (element.commandId === SystemConstants.UPDATE_ACTION) {
            this.selectedUpdates.push(element.functionId);
          }
          if (element.commandId === SystemConstants.DELETE_ACTION) {
            this.selectedDeletes.push(element.functionId);
          }
          if (element.commandId === SystemConstants.VIEW_ACTION) {
            this.selectedViews.push(element.functionId);
          }
          if (element.commandId === SystemConstants.APPROVE_ACTION) {
            this.selectedApproves.push(element.functionId);
          }
          setTimeout(() => { this.blockedPanel = false; }, 1000);
        });

      }, error => {
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }
  loadAllRoles() {
    this.blockedPanel = true;
    this.subscription.add(this.rolesService.getAll()
      .subscribe((response: any) => {
        this.roles = response;
        setTimeout(() => { this.blockedPanel = false; }, 1000);
      }));
  }

  ngOnDestroy(): void {
    this.subscription.unsubscribe();
  }
}
