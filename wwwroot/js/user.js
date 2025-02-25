const uri = '/User';
const token = sessionStorage.getItem('token'); // מקבל את ה-token ששמרת ב-sessionStorage

if (token) {
    const payload = token.split('.')[1];
    const decodedPayload = atob(payload);
    const jsonPayload = JSON.parse(decodedPayload);
 //   const userId = jsonPayload.UserId;
    const userId = parseInt(jsonPayload.UserId, 10);
    jsonPayload.UserId=userId;
    showUserName(userId);

}
function showUserName(id){
    const userName = document.getElementById('userName');
    fetch(`${uri}/${id}`,{
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
    .then(response=>{
        if (!response.ok) {
            throw new Error("הגישה נדחתה! ייתכן שהמשתמש אינו מחובר.");
        }
        return response.json();
    })
    .then(data => {
        console.log(data);
        userName.innerHTML=data.username});
}

function getItems() {
    fetch(uri, {
        method: 'GET',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json'
        }
    })
        .then(response => {
            if (!response.ok) {
                throw new Error("הגישה נדחתה! ייתכן שהמשתמש אינו מחובר.");
            }
            return response.json();
        })
        .then(data => _displayItems(data)) 
        .catch(error => alert(error.message));
        
}

function addItem() {
    const addUsernameTextbox = document.getElementById('add-username');
    const addPasswordTextbox = document.getElementById('add-password');
    const addRoleCheckbox = document.getElementById('add-role');

    const item = {
        username: addUsernameTextbox.value.trim(),
        password: addPasswordTextbox.value.trim(),
        isAdmin: addRoleCheckbox.checked
    };

    fetch(uri, {
        method: 'POST',
        headers: {
            'Authorization': `Bearer ${token}`,
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(item)
    })
        .then(response => response.json())
        .then(() => {
            getItems();
            addUsernameTextbox.value = '';
            addPasswordTextbox.value = '';
            addRoleCheckbox.checked = false;
        })
        .catch(error => console.error('Unable to add item.', error));
}

function deleteItem(id) {
    fetch(`${uri}/${id}`, {
        method: 'DELETE',
        headers: {
            'Authorization': `Bearer ${token}`
        }
    })
        .then(() => getItems())
        .catch(error => console.error('Unable to delete item.', error));
}

function _displayItems(data) {
    const tBody = document.getElementById('users');
    tBody.innerHTML = '';

    const button = document.createElement('button');
     
    data.forEach(item => {
        let role = document.createElement('label');
        role.textContent = item.isAdmin ? 'Admin' : 'User';
        let deleteButton = button.cloneNode(false);
        deleteButton.innerText = 'Delete';
        deleteButton.setAttribute('onclick', `deleteItem(${item.userId})`);

        let tr = tBody.insertRow();

        let td1 = tr.insertCell(0);
        td1.appendChild(document.createTextNode(item.userId));

        let td2 = tr.insertCell(1);
        td2.appendChild(document.createTextNode(item.username));

        let td3 = tr.insertCell(2);
        td3.appendChild(document.createTextNode(item.password));

        let td4 = tr.insertCell(3);
        td4.appendChild(role);

        let td5 = tr.insertCell(4);
        td5.appendChild(deleteButton);
    });
}

// התחל את הצגת המשתמשים
getItems();
