function FormSelectField(val, data_var) {
    let color_name = val.textContent
    $(`[${data_var}=""]`).html(`
    <option value="${color_name}"> ${color_name} </option>
    `)
}
