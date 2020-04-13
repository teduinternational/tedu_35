import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { BaseService } from './base.service';
import { catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { Function } from '../models';

@Injectable({ providedIn: 'root' })
export class UtilitiesService extends BaseService {
  constructor(private http: HttpClient) {
    super();
  }
  UnflatteringForLeftMenu = (arr: any[]): any[] => {
    const map = {};
    const roots: any[] = [];
    for (let i = 0; i < arr.length; i++) {
      const node = arr[i];
      node.children = [];
      map[node.id] = i; // use map to look-up the parents
      if (node.parentId !== null) {
        delete node['children'];
        arr[map[node.parentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }

  UnflatteringForTree = (arr: any[]): any[] => {
    const map = {};
    const roots: any[] = [];
    let node = {
      data: {
        id: '',
        parentId: ''
      },
      expanded: true,
      children: []
    };
    for (let i = 0; i < arr.length; i += 1) {
      map[arr[i].id] = i; // initialize the map
      arr[i].data = arr[i]; // initialize the data
      arr[i].children = []; // initialize the children
    }
    for (let i = 0; i < arr.length; i += 1) {
      node = arr[i];
      if (node.data.parentId !== null && arr[map[node.data.parentId]] != undefined) {
        arr[map[node.data.parentId]].children.push(node);
      } else {
        roots.push(node);
      }
    }
    return roots;
  }
}