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

  MakeSeoTitle(input: string) {
    if (input == undefined || input == '') {
      return '';
    }
    // Đổi chữ hoa thành chữ thường
    let slug = input.toLowerCase();

    // Đổi ký tự có dấu thành không dấu
    slug = slug.replace(/á|à|ả|ạ|ã|ă|ắ|ằ|ẳ|ẵ|ặ|â|ấ|ầ|ẩ|ẫ|ậ/gi, 'a');
    slug = slug.replace(/é|è|ẻ|ẽ|ẹ|ê|ế|ề|ể|ễ|ệ/gi, 'e');
    slug = slug.replace(/i|í|ì|ỉ|ĩ|ị/gi, 'i');
    slug = slug.replace(/ó|ò|ỏ|õ|ọ|ô|ố|ồ|ổ|ỗ|ộ|ơ|ớ|ờ|ở|ỡ|ợ/gi, 'o');
    slug = slug.replace(/ú|ù|ủ|ũ|ụ|ư|ứ|ừ|ử|ữ|ự/gi, 'u');
    slug = slug.replace(/ý|ỳ|ỷ|ỹ|ỵ/gi, 'y');
    slug = slug.replace(/đ/gi, 'd');
    // Xóa các ký tự đặt biệt
    slug = slug.replace(/\`|\~|\!|\@|\#|\||\$|\%|\^|\&|\*|\(|\)|\+|\=|\,|\.|\/|\?|\>|\<|\'|\"|\:|\;|_/gi, '');
    // Đổi khoảng trắng thành ký tự gạch ngang
    slug = slug.replace(/ /gi, '-');
    // Đổi nhiều ký tự gạch ngang liên tiếp thành 1 ký tự gạch ngang
    // Phòng trường hợp người nhập vào quá nhiều ký tự trắng
    slug = slug.replace(/\-\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-\-/gi, '-');
    slug = slug.replace(/\-\-\-/gi, '-');
    slug = slug.replace(/\-\-/gi, '-');
    // Xóa các ký tự gạch ngang ở đầu và cuối
    slug = '@' + slug + '@';
    slug = slug.replace(/\@\-|\-\@|\@/gi, '');

    return slug;
  }

  ToFormData(formValue: any) {
    const formData = new FormData();
    for (const key of Object.keys(formValue)) {
      const value = formValue[key];
      formData.append(key, value);
    }

    return formData;
  }
}