import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Component } from '@angular/core';
import * as CryptoJS from 'crypto-js';
import { environment } from 'src/environments/environment';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import Swal from 'sweetalert2';


interface _Request {
  EncryptedData: string;
  EncryptionKey: string;
}

interface TransactionResponse {
  responseCode: string;
  message: string;
  approvalCode: string;
  dateTime: string;
}

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  private baseUrl = `${environment.baseUrl}`;
  authForm: FormGroup;
  infoForm: FormGroup;

  auth = {
    cardNo: 0,
    password: ""
  };
  transaction: any = {
    processingCode: "",
    systemTraceNr: 0,
    functionCode: 0,
    cardNo: 0,
    cardHolder: "",
    amountTrxn: 0,
    currencyCode: 0,
  };
  
  cardNo1 = 0;
  cardNo2 = 0;
  cardNo3 = 0;
  cardNo4 = 0;
  authenticated = false;
  token: string;
  generatedEncryptionKey: string;
  result: TransactionResponse | null = null;
  request: _Request | null = null;

  constructor(private http: HttpClient, private formBuilder: FormBuilder) {

  }


  ngOnInit() {
    this.generateEncryptionKey();
    this.authForm = this.formBuilder.group({
      authCardNo1: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      authCardNo2: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      authCardNo3: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      authCardNo4: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      password: ['', [Validators.required, Validators.maxLength(4)]]
    });
    this.infoForm = this.formBuilder.group({
      cardNo1: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      cardNo2: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      cardNo3: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      cardNo4: ['', [Validators.required, Validators.pattern('\\d{4}')]],
      processingCode: ['', [Validators.required]],
      systemTraceNr: ['', [Validators.required]],
      functionCode: ['', [Validators.required]],
      cardNo: ['', [Validators.required]],
      cardHolder: ['', [Validators.required]],
      amountTrxn: ['', [Validators.required]],
      currencyCode: ['', [Validators.required]],
      encryptionKey: ['', [Validators.required]],
    });
  }
  authService() {
    if (this.authForm.valid) {
      let cardNo = parseInt(this.authForm.value.authCardNo1 + "" + this.authForm.value.authCardNo2 + "" + this.authForm.value.authCardNo3 + "" + this.authForm.value.authCardNo4);
      this.auth.cardNo = cardNo;
      this.auth.password = this.authForm.value.password;
      const plaintext = JSON.stringify(this.auth);
      const _encryptedData = this.encryptData(plaintext, this.generatedEncryptionKey);
      this.http.post<any>(`${this.baseUrl}/api/authentication/login`,
        { encryptedData: _encryptedData, encryptionKey: this.generatedEncryptionKey }).subscribe(
          (res) => {
            this.token = res.token;
            this.authenticated = true;
          },
          (res) => {
            Swal.fire({
              icon: 'error',
              title: 'Error',
              text: res.error?.message,
              confirmButtonText: 'OK'
            });
          }
        );
    }
    else {
      Swal.fire({
        icon: 'error',
        title: 'Error',
        text: "Enter Valid CardNo && Password",
        confirmButtonText: 'OK'
      })
    }
  }


  generateEncryptionKey() {
    this.http.post(`${this.baseUrl}/api/transaction/generateEncryptionKey`, {}).subscribe(
      (res: any) => {
        this.generatedEncryptionKey = res.encryptionKey;

      });
  }

  processTransaction() {
    const headers = new HttpHeaders().set('Authorization', "Bearer " + this.token);
    this.transaction.processingCode = this.infoForm.value.processingCode;
    this.transaction.systemTraceNr = this.infoForm.value.systemTraceNr;
    this.transaction.functionCode = this.infoForm.value.functionCode;
    this.transaction.cardHolder = this.infoForm.value.cardHolder;
    this.transaction.amountTrxn = this.infoForm.value.amountTrxn;
    this.transaction.currencyCode = this.infoForm.value.currencyCode;
    this.transaction.encryptionKey = this.infoForm.value.encryptionKey;
    // Combine 4 Input CardNo
    let cardNo = parseInt(this.infoForm.value.cardNo1 + "" + this.infoForm.value.cardNo2 + "" + this.infoForm.value.cardNo3 + "" + this.infoForm.value.cardNo4);
    this.transaction.cardNo = cardNo;
    const jsonData = JSON.stringify(this.transaction);
    const encryptedData = this.encryptData(JSON.stringify(jsonData), this.generatedEncryptionKey).toString();
    // Make the API request with the encrypted data
    this.http.post<any>(`${this.baseUrl}/api/transaction/processTransaction`,
      { encryptedData: encryptedData, encryptionKey: this.generatedEncryptionKey }, { headers }).subscribe(
        (response) => {
          // Decrypt the response data using the encryption key
          console.log(response.encryptedResponse);

          const encryptedData = this.decryptData(response.encryptedResponse, this.generatedEncryptionKey);

          // Parse the decrypted JSON data into a TransactionResponse object
          let _result = JSON.parse(encryptedData);
          this.result = _result;

        },
        (res) => {
          Swal.fire({
            icon: 'error',
            title: 'Error',
            text: res.error?.message,
            confirmButtonText: 'OK'
          });
        }
      );
  }


  encryptData = (plaintext: string, secretKey: string): string => {

    const key = CryptoJS.enc.Utf8.parse(secretKey);
    const iv = CryptoJS.lib.WordArray.random(16);

    const encryptedData = CryptoJS.AES.encrypt(plaintext, key, {
      iv: iv,
      mode: CryptoJS.mode.CBC,
      padding: CryptoJS.pad.Pkcs7
    });

    const combinedData = iv.concat(encryptedData.ciphertext);
    const encryptedText = combinedData.toString(CryptoJS.enc.Base64);
    return encryptedText;
  };
  decryptData(encryptedData: string, encryptionKey: string): string {
    try {
      const key = CryptoJS.enc.Utf8.parse(encryptionKey);

      // Decode the Base64 string
      const encryptedBytes = CryptoJS.enc.Base64.parse(encryptedData);

      // Extract the IV from the encrypted data
      const iv = encryptedBytes.clone();
      iv.sigBytes = 16;
      iv.clamp();

      // Extract the encrypted message
      const ciphertext = encryptedBytes.clone();
      ciphertext.words.splice(0, 4); // Remove IV
      ciphertext.sigBytes -= 16; // Adjust length

      // Decrypt the data
      const decryptedData = CryptoJS.AES.decrypt({ ciphertext }, key, { iv: iv });

      // Convert the decrypted data to a UTF-8 string
      const decryptedText = decryptedData.toString(CryptoJS.enc.Utf8);

      return decryptedText;
    } catch (ex) {
      throw new Error('Decryption failed. Error: ' + ex.message);
    }
  }


  resetAuthForm() {
    // Reset the auth form fields
    this.authForm.reset();
  }

  resetInfoForm() {
    // Reset the values of the information form fields
    this.transaction.processingCode = null;
    this.transaction.systemTraceNr = null;
    this.transaction.functionCode = null;
    this.transaction.cardHolder = null;
    this.cardNo1 = null;
    this.cardNo2 = null;
    this.cardNo3 = null;
    this.cardNo4 = null;
    this.transaction.amountTrxn = null;
    this.transaction.currencyCode = null;
  }
}
